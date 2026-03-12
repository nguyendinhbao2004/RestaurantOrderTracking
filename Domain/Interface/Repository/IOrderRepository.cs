using RestaurantOrderTracking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantOrderTracking.Domain.Interface.Repository
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        Task<(IEnumerable<Order>, int totalCount)> GetPagedOrdersAsync(string? keyword, int pageIndex, int pageSize);
        Task<bool> TableHasActiveOrder(Guid tableId);
        Task<Order?> GetOrderByIdWithDetailsAsync(Guid orderId);
    }
}
