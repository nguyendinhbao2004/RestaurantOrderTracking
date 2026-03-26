using MediatR;
using RestaurantOrderTracking.Application.Dto.OrderItem;
using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Interface.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RestaurantOrderTracking.Application.Feature.OrderItem.Queries.GetOrderItemsByAccount
{
    public class GetOrderItemsByAccountHandler : IRequestHandler<GetOrderItemsByAccountQuery, Result<IEnumerable<OrderItemByAccountResponse>>>
    {
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IAccountRepository _accountRepository;

        public GetOrderItemsByAccountHandler(IOrderItemRepository orderItemRepository, IAccountRepository accountRepository)
        {
            _orderItemRepository = orderItemRepository;
            _accountRepository = accountRepository;
        }

        public async Task<Result<IEnumerable<OrderItemByAccountResponse>>> Handle(GetOrderItemsByAccountQuery request, CancellationToken cancellationToken)
        {
            var account = await _accountRepository.GetByIdAsync(request.AccountId, cancellationToken);
            if (account == null)
            {
                return Result<IEnumerable<OrderItemByAccountResponse>>.Failure("Account not found.");
            }

            var orderItems = await _orderItemRepository.GetOrderItemsForAccountAsync(account.RoleId, request.AccountId);

            var accountIds = orderItems
                .Where(x => !string.IsNullOrEmpty(x.CreatedBy) && Guid.TryParse(x.CreatedBy, out _))
                .Select(x => Guid.Parse(x.CreatedBy!))
                .Distinct()
                .ToList();

            var accountDict = new Dictionary<string, string>();
            if (accountIds.Any())
            {
                foreach (var id in accountIds)
                {
                    var acc = await _accountRepository.GetByIdAsync(id, cancellationToken);
                    if (acc != null)
                    {
                        accountDict[id.ToString()] = acc.FullName;
                    }
                }
            }

            var responseList = new List<OrderItemByAccountResponse>();

            foreach (var item in orderItems)
            {
                var response = new OrderItemByAccountResponse
                {
                    OrderItemId = item.Id,
                    OrderId = item.OrderId,
                    ProductId = item.ProductId,
                    ProductName = item.Product?.Name ?? "Unknown Product",
                    Note = item.Note,
                    Status = ((int)item.Status).ToString(),
                    OrderAt = item.CreatedAt,
                    CreatedBy = item.CreatedBy,
                    CreatedByName = null,
                    TableId = item.Order?.TableId,
                    TableNumber = item.Order?.Table?.TableNumber,
                    AreaId = item.Order?.Table?.AreaId,
                    AreaName = item.Order?.Table?.Area?.Name,
                    OrderType = item.Order?.OrderTypes.ToString(),
                    OrderStatus = item.Order != null ? ((int)item.Order.Status).ToString() : null
                };

                if (!string.IsNullOrWhiteSpace(item.CreatedBy) && accountDict.TryGetValue(item.CreatedBy, out var fullName))
                {
                    response.CreatedByName = fullName;
                }
                else
                {
                    if (string.Equals(item.OrderChannel, "Online", StringComparison.OrdinalIgnoreCase))
                    {
                        response.CreatedByName = "Khách đặt online";
                    }
                    else if (string.Equals(item.OrderChannel, "QR", StringComparison.OrdinalIgnoreCase))
                    {
                        response.CreatedByName = "Khách tự order";
                    }
                }

                // If Status format needs to be string like "Confirmed", change this logic.
                // Based on standard implementation, it might just return parsed string enum.
                response.Status = ((int)item.Status).ToString();

                responseList.Add(response);
            }

            return Result<IEnumerable<OrderItemByAccountResponse>>.Success("Get list of order items successfully.", responseList);
        }
    }
}
