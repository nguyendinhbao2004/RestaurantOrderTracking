using AutoMapper;
using MediatR;
using RestaurantOrderTracking.Application.Dto.Order;
using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Interface.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantOrderTracking.Application.Feature.Order.Queries.GetAllOrder
{
    public class GetAllOrderHandler : IRequestHandler<GetAllOrderQueries, PagedResult<OrderResponse>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        public GetAllOrderHandler(IOrderRepository orderRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
        }
        public async Task<PagedResult<OrderResponse>> Handle(GetAllOrderQueries request, CancellationToken cancellationToken)
        {
            var (orders, totalCount) = await _orderRepository.GetPagedOrdersAsync(request.Keyword, request.PageIndex, request.PageSize);
            var orderResponses = _mapper.Map<List<OrderResponse>>(orders).ToList();
            return new PagedResult<OrderResponse>(orderResponses, request.PageIndex, request.PageSize, totalCount, "Get Order Successfull");
        }
    }
}
