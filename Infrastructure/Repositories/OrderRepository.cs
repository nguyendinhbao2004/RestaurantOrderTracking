using Microsoft.EntityFrameworkCore;
using RestaurantOrderTracking.Domain.Entities;
using RestaurantOrderTracking.Domain.Enums;
using RestaurantOrderTracking.Domain.Interface.Repository;
using RestaurantOrderTracking.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantOrderTracking.Infrastructure.Repositories
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        public OrderRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<(IEnumerable<Order>, int totalCount)> GetPagedOrdersAsync(string? keyword, int pageIndex, int pageSize)
        {
            var query = _dbSet.AsQueryable();
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(p => p.Table.TableNumber.Contains(keyword));
            }
            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return (items, totalCount);
        }

        public async Task<bool> TableHasActiveOrder(Guid tableId)
        {
            return await _dbSet.AnyAsync(o =>
                o.TableId == tableId &&
                (o.Status == OrderStatus.Confirmed || o.Status == OrderStatus.Paying));
        }
    }
}
