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
            var query = _dbSet.Include(o => o.Table).AsQueryable();
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(p => p.Table.TableNumber.Contains(keyword));
            }
            
            var items = await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            var totalCount = await query.CountAsync();
            return (items, totalCount);
        }

        public async Task<bool> TableHasActiveOrder(Guid tableId)
        {
            return await _dbSet.AnyAsync(o =>
                o.TableId == tableId &&
                (o.Status == OrderStatus.Confirmed ||
                 o.Status == OrderStatus.Preparing ||
                 o.Status == OrderStatus.Paying));
        }

        public async Task<Order?> GetOrderByIdWithDetailsAsync(Guid orderId)
        {
            return await _dbSet
                .Include(o => o.Table)
                .Include(o => o.Waiter)
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.ChefAccount)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.WaiterAccount)
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }
        public async Task<int> GetTotalOrdersCountAsync()
        {
            return await _dbSet.CountAsync();
        }

        public async Task<int> GetPendingOrdersCountAsync()
        {
            return await _dbSet.CountAsync(o => o.Status == OrderStatus.Pending);
        }
    }
}

