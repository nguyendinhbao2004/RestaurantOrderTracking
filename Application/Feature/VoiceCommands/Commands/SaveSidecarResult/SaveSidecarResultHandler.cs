using MediatR;
using RestaurantOrderTracking.Application.Common.Interface;
using RestaurantOrderTracking.Application.Feature.VoiceCommands.Dtos;
using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Entities;
using RestaurantOrderTracking.Domain.Enums;
using RestaurantOrderTracking.Domain.Interface;
using RestaurantOrderTracking.Domain.Interface.Repository;
using Domain.Interface.Repository;
using OrderEntity = RestaurantOrderTracking.Domain.Entities.Order;

namespace RestaurantOrderTracking.Application.Feature.VoiceCommands.Commands.SaveSidecarResult
{
    public class SaveSidecarResultHandler : IRequestHandler<SaveSidecarResultCommand, Result<Guid>>
    {
        private readonly IGenericRepository<VoiceCommand> _voiceCommandRepository;
        private readonly ITableRepository _tableRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IVoiceCommandAiParser _voiceCommandAiParser;
        private readonly IUnitOfWork _unitOfWork;

        public SaveSidecarResultHandler(
            IGenericRepository<VoiceCommand> voiceCommandRepository,
            ITableRepository tableRepository,
            IOrderRepository orderRepository,
            IProductRepository productRepository,
            IVoiceCommandAiParser voiceCommandAiParser,
            IUnitOfWork unitOfWork)
        {
            _voiceCommandRepository = voiceCommandRepository;
            _tableRepository = tableRepository;
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _voiceCommandAiParser = voiceCommandAiParser;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(SaveSidecarResultCommand request, CancellationToken cancellationToken)
        {
            var voiceCommand = await _voiceCommandRepository.GetByIdAsync(request.VoiceCommandId, cancellationToken);
            if (voiceCommand is null)
            {
                return Result<Guid>.Failure("VoiceCommand not found.");
            }

            if (!string.IsNullOrWhiteSpace(request.ErrorMessage))
            {
                voiceCommand.MarkAsFailed(request.ErrorMessage);
                _voiceCommandRepository.Update(voiceCommand, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result<Guid>.Success("Voice result saved as failed.", voiceCommand.Id);
            }

            if (string.IsNullOrWhiteSpace(request.TranscribedText))
            {
                return Result<Guid>.Failure("TranscribedText is required when ErrorMessage is empty.");
            }

            voiceCommand.SetTranscription(request.TranscribedText, request.ConfidenceScore ?? 0f);
            voiceCommand.MarkAsCompleted();

            var parsed = await _voiceCommandAiParser.ParseAsync(request.TranscribedText, cancellationToken);
            voiceCommand.SetParsedResult(null, parsed.Intent);

            if (ShouldCreateOrderItems(parsed))
            {
                var table = await ResolveTableByNumberAsync(parsed.TableNumber!, cancellationToken);
                if (table is null)
                {
                    return Result<Guid>.Failure($"Table '{parsed.TableNumber}' not found.");
                }

                voiceCommand.SetParsedResult(table.Id, parsed.Intent);

                var productList = await _productRepository.GetAllAsync();
                var resolvedItems = ResolveProducts(parsed.Items, productList);
                if (resolvedItems.Count == 0)
                {
                    return Result<Guid>.Failure("No product can be matched from voice text.");
                }

                voiceCommand.ParsedProductName = string.Join(", ",
                    resolvedItems.Select(i => i.Product.Name).Distinct(StringComparer.OrdinalIgnoreCase));

                var order = GetActiveOrder(table)
                    ?? await CreateDineInOrderAsync(table, voiceCommand.AccountId, cancellationToken);

                foreach (var resolved in resolvedItems)
                {
                    var quantity = Math.Max(1, resolved.Quantity);
                    for (var i = 0; i < quantity; i++)
                    {
                        order.AddItem(
                            productId: resolved.Product.Id,
                            accountId: voiceCommand.AccountId,
                            note: parsed.Note ?? string.Empty,
                            orderChannel: "voice-ai");
                    }
                }
            }

            _voiceCommandRepository.Update(voiceCommand, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success("Voice result saved.", voiceCommand.Id);
        }

        private static bool ShouldCreateOrderItems(ParsedVoiceCommandDto parsed)
        {
            return string.Equals(parsed.Intent, "add_item", StringComparison.OrdinalIgnoreCase)
                && !string.IsNullOrWhiteSpace(parsed.TableNumber)
                && parsed.Items.Any();
        }

        private async Task<Table?> ResolveTableByNumberAsync(string tableNumber, CancellationToken cancellationToken)
        {
            var normalizedInput = NormalizeTableNumber(tableNumber);
            var allTables = await _tableRepository.GetAllAsync();

            var table = allTables.FirstOrDefault(t =>
                string.Equals(NormalizeTableNumber(t.TableNumber), normalizedInput, StringComparison.OrdinalIgnoreCase));

            if (table is null)
            {
                return null;
            }

            return await _tableRepository.GetByIdAsync(table.Id);
        }

        private static string NormalizeTableNumber(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
            {
                return string.Empty;
            }

            return raw.Trim()
                .Replace("table", string.Empty, StringComparison.OrdinalIgnoreCase)
                .Replace("ban", string.Empty, StringComparison.OrdinalIgnoreCase)
                .Trim();
        }

        private static List<ResolvedVoiceItem> ResolveProducts(
            IEnumerable<ParsedVoiceItemDto> parsedItems,
            IEnumerable<Product> products)
        {
            var productArray = products.Where(p => p.IsActive).ToArray();
            var resolved = new List<ResolvedVoiceItem>();

            foreach (var parsedItem in parsedItems)
            {
                if (string.IsNullOrWhiteSpace(parsedItem.ProductName))
                {
                    continue;
                }

                var product = FindBestProduct(parsedItem.ProductName, productArray);
                if (product is null)
                {
                    continue;
                }

                resolved.Add(new ResolvedVoiceItem(product, parsedItem.Quantity));
            }

            return resolved;
        }

        private static Product? FindBestProduct(string productName, IEnumerable<Product> products)
        {
            var trimmed = productName.Trim();

            var exact = products.FirstOrDefault(p =>
                string.Equals(p.Name, trimmed, StringComparison.OrdinalIgnoreCase));
            if (exact is not null)
            {
                return exact;
            }

            return products.FirstOrDefault(p =>
                p.Name.Contains(trimmed, StringComparison.OrdinalIgnoreCase)
                || trimmed.Contains(p.Name, StringComparison.OrdinalIgnoreCase));
        }

        private static OrderEntity? GetActiveOrder(Table table)
        {
            return table.Orders
                .Where(o => o.Status == OrderStatus.Confirmed || o.Status == OrderStatus.Preparing)
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefault();
        }

        private async Task<OrderEntity> CreateDineInOrderAsync(Table table, Guid accountId, CancellationToken cancellationToken)
        {
            var order = new OrderEntity(table.Id, OrderType.DineIn, accountId);
            await _orderRepository.AddAsync(order);

            table.SetOccupied();
            _tableRepository.Update(table, cancellationToken);

            return order;
        }

        private sealed record ResolvedVoiceItem(Product Product, int Quantity);
    }
}
