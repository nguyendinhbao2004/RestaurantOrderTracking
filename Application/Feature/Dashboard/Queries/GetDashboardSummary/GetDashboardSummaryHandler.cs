using Application.Dto.Dashboard;
using MediatR;
using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Interface.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Feature.Dashboard.Queries.GetDashboardSummary
{
    public class GetDashboardSummaryHandler : IRequestHandler<GetDashboardSummaryQueries, Result<DashboardSummaryResponse>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IBillRepository _billRepository;

        public GetDashboardSummaryHandler(IOrderRepository orderRepository, IBillRepository billRepository)
        {
            _orderRepository = orderRepository;
            _billRepository = billRepository;
        }

        public async Task<Result<DashboardSummaryResponse>> Handle(GetDashboardSummaryQueries request, CancellationToken cancellationToken)
        {
            var totalOrdersCount = await _orderRepository.GetTotalOrdersCountAsync();
            var pendingOrdersCount = await _orderRepository.GetPendingOrdersCountAsync();
            var totalRevenue = await _billRepository.GetTotalRevenueAsync();

            decimal avgOrderValue = totalOrdersCount > 0 ? totalRevenue / totalOrdersCount : 0;

            var response = new DashboardSummaryResponse
            {
                TotalOrders = totalOrdersCount,
                TotalRevenue = totalRevenue,
                AvgOrderValue = avgOrderValue,
                PendingOrders = pendingOrdersCount
            };

            return new Result<DashboardSummaryResponse>(true, "Get Dashboard Summary Successfully", null, response);
        }
    }
}
