using RestaurantOrderTracking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RestaurantOrderTracking.Domain.Interface.Repository
{
    public interface IBillRepository : IGenericRepository<Bill>
    {
        Task<(IEnumerable<Bill>, int totalCount)> GetPagedBillsAsync(string? keyword, int pageIndex, int pageSize);
        Task<Bill?> GetByIdWithDetailsAsync(Guid billId);
        Task<Bill?> GetByOrderIdAsync(Guid orderId);
        Task<decimal> GetTotalRevenueAsync();
    }
}
