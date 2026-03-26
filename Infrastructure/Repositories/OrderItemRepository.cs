using Microsoft.EntityFrameworkCore;
using RestaurantOrderTracking.Domain.Entities;
using RestaurantOrderTracking.Domain.Enums;
using RestaurantOrderTracking.Domain.Interface.Repository;
using RestaurantOrderTracking.Infrastructure.Data;

namespace RestaurantOrderTracking.Infrastructure.Repositories
{
    public class OrderItemRepository : GenericRepository<OrderItem>, IOrderItemRepository
    {
        public OrderItemRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<(IEnumerable<OrderItem>, int totalCount)> GetPagedOrderItemsAsync(string? keyword, int pageIndex, int pageSize)
        {
            var query = _dbSet
                .Include(oi => oi.Product)
                .Include(oi => oi.Order)
                    .ThenInclude(o => o.Table)
                .AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(oi => oi.Product.Name.Contains(keyword));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(oi => oi.CreatedAt)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        // get order items by status with pagination
        public async Task<(IEnumerable<OrderItem>, int totalCount)> GetPagedOrderItemsByStatusAsync(OrderItemStatus status, int pageIndex, int pageSize)
        {
            var query = _dbSet
                .Include(oi => oi.Product)
                .Include(oi => oi.Order)
                    .ThenInclude(o => o.Table)
                        .ThenInclude(t => t.Area)
                .Where(oi => oi.Status == status)
                .AsQueryable();

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(oi => oi.CreatedAt)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<IEnumerable<OrderItem>> GetOrderItemsByStatusAsync(RestaurantOrderTracking.Domain.Enums.OrderItemStatus status)
        {
            return await _dbSet
                .Include(oi => oi.Order)
                    .ThenInclude(o => o.Table)
                        .ThenInclude(t => t.Area)
                .Include(oi => oi.Product)
                .Include(oi => oi.ChefAccount)
                .Include(oi => oi.WaiterAccount)
                .Where(oi => oi.Status == status)
                .OrderByDescending(oi => oi.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<OrderItem>> GetOrderItemsForAccountAsync(int roleId, Guid accountId)
        {
            var query = _dbSet
                .Include(oi => oi.Product)
                .AsQueryable();

            if (roleId == 6)
            {
                query = query.Where(oi => oi.Status == RestaurantOrderTracking.Domain.Enums.OrderItemStatus.Confirmed || oi.Status == RestaurantOrderTracking.Domain.Enums.OrderItemStatus.Cooking);
            }
            else if (roleId == 3)
            {
                query = query.Where(oi => oi.Status == RestaurantOrderTracking.Domain.Enums.OrderItemStatus.Cooking && oi.ChefAccountId == accountId);
            }
            else
            {
                return new List<OrderItem>();
            }

            return await query
                .OrderByDescending(oi => oi.CreatedAt)
                .ToListAsync();
        }
    }
}
