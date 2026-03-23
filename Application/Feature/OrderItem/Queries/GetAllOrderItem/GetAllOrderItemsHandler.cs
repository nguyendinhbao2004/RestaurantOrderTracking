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

            var accountIds = orderItemResponses
                .Where(r => !string.IsNullOrEmpty(r.CreatedBy) && Guid.TryParse(r.CreatedBy, out _))
                .Select(r => Guid.Parse(r.CreatedBy))
                .Distinct()
                .ToList();

            if (accountIds.Any())
            {
                var accountDict = new Dictionary<string, string>();
                foreach (var id in accountIds)
                {
                    var account = await _accountRepository.GetByIdAsync(id, cancellationToken);
                    if (account != null)
                    {
                        accountDict[id.ToString()] = account.FullName;
                    }
                }

                foreach (var response in orderItemResponses)
                {
                    if (!string.IsNullOrEmpty(response.CreatedBy) && accountDict.TryGetValue(response.CreatedBy, out var fullName))
                    {
                        response.CreatedByName = fullName;
                    }
                }
            }

            return new PagedResult<OrderItemResponse>(orderItemResponses, request.PageIndex, request.PageSize, totalCount, "Get Order Items Successful");
        }
    }
}
