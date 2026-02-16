using RestaurantOrderTracking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantOrderTracking.Domain.Interface.Repository
{
    public interface ITableRepository : IGenericRepository<Table>
    {
        Task<bool> IsTableAvailableAsync(int tableNumber);
        Task<bool> IsOccupedAsync(int tableNumber);
        Task<(IEnumerable<Table>, int totalCount)> GetPagedTablesAsync(string? keyword, int pageIndex, int pageSize);
        Task<(IEnumerable<Table>, int totalCount)> GetByIdAsync(Guid id, int pageIndex, int pageSize);
    }
}
