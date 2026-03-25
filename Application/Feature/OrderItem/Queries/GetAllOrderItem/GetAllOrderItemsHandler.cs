using AutoMapper;
using MediatR;
using RestaurantOrderTracking.Application.Dto.OrderItem;
using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Interface.Repository;

namespace RestaurantOrderTracking.Application.Feature.OrderItem.Queries.GetAllOrderItem
{
    public class GetAllOrderItemsHandler : IRequestHandler<GetAllOrderItemsQuery, PagedResult<OrderItemResponse>>
    {
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IMapper _mapper;
        private readonly IAccountRepository _accountRepository;

        public GetAllOrderItemsHandler(
            IOrderItemRepository orderItemRepository, 
            IMapper mapper, 
            IAccountRepository accountRepository)
        {
            _orderItemRepository = orderItemRepository;
            _mapper = mapper;
            _accountRepository = accountRepository;
        }

        public async Task<PagedResult<OrderItemResponse>> Handle(GetAllOrderItemsQuery request, CancellationToken cancellationToken)
        {
            var (orderItems, totalCount) = await _orderItemRepository.GetPagedOrderItemsAsync(request.Keyword, request.PageIndex, request.PageSize);
            var orderItemResponses = _mapper.Map<List<OrderItemResponse>>(orderItems).ToList();
            var orderChannelByOrderItemId = orderItems.ToDictionary(x => x.Id, x => x.OrderChannel);

            var accountIds = orderItemResponses
                .Where(r => !string.IsNullOrEmpty(r.CreatedBy) && Guid.TryParse(r.CreatedBy, out _))
                .Select(r => Guid.Parse(r.CreatedBy!))
                .Distinct()
                .ToList();

            var accountDict = new Dictionary<string, string>();
            if (accountIds.Any())
            {
                foreach (var id in accountIds)
                {
                    var account = await _accountRepository.GetByIdAsync(id, cancellationToken);
                    if (account != null)
                    {
                        accountDict[id.ToString()] = account.FullName;
                    }
                }
            }

            foreach (var response in orderItemResponses)
            {
                // Nếu có account → lấy tên
                if (!string.IsNullOrWhiteSpace(response.CreatedBy) && accountDict.TryGetValue(response.CreatedBy, out var fullName))
                {
                    response.CreatedByName = fullName;
                    continue;
                }

                // Nếu không có account → kiểm tra OrderChannel
                if (!orderChannelByOrderItemId.TryGetValue(response.Id, out var orderChannel))
                {
                    continue;
                }

                if (string.Equals(orderChannel, "Online", StringComparison.OrdinalIgnoreCase))
                {
                    response.CreatedByName = "Khách đặt online";
                }
                else if (string.Equals(orderChannel, "QR", StringComparison.OrdinalIgnoreCase))
                {
                    response.CreatedByName = "Khách tự order";
                }
            }

            return new PagedResult<OrderItemResponse>(orderItemResponses, request.PageIndex, request.PageSize, totalCount, "Get Order Items Successful");
        }
    }
}
