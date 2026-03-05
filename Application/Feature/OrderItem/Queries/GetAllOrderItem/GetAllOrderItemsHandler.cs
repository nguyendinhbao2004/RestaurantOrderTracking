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

        public GetAllOrderItemsHandler(IOrderItemRepository orderItemRepository, IMapper mapper)
        {
            _orderItemRepository = orderItemRepository;
            _mapper = mapper;
        }

        public async Task<PagedResult<OrderItemResponse>> Handle(GetAllOrderItemsQuery request, CancellationToken cancellationToken)
        {
            var (orderItems, totalCount) = await _orderItemRepository.GetPagedOrderItemsAsync(request.Keyword, request.PageIndex, request.PageSize);
            var orderItemResponses = _mapper.Map<List<OrderItemResponse>>(orderItems).ToList();
            return new PagedResult<OrderItemResponse>(orderItemResponses, totalCount, request.PageIndex, request.PageSize, "Get Order Items Successful");
        }
    }
}
