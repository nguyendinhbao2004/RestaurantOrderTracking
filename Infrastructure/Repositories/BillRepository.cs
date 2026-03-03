using Microsoft.EntityFrameworkCore;
using RestaurantOrderTracking.Domain.Entities;
using RestaurantOrderTracking.Domain.Interface.Repository;
using RestaurantOrderTracking.Infrastructure.Data;

namespace RestaurantOrderTracking.Infrastructure.Repositories
{
    public class BillRepository : GenericRepository<Bill>, IBillRepository
    {
        public BillRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<(IEnumerable<Bill>, int totalCount)> GetPagedBillsAsync(string? keyword, int pageIndex, int pageSize)
        {
            var query = _dbSet
                .Include(b => b.Order)
                    .ThenInclude(o => o.Table)
                .Include(b => b.Account)
                .AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(b =>
                    b.Order.Table.TableNumber.Contains(keyword) ||
                    b.Account.FullName.Contains(keyword) ||
                    b.Status.ToString().Contains(keyword));
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(b => b.CreatedAt)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<Bill?> GetByIdWithDetailsAsync(Guid billId)
        {
            return await _dbSet
                .Include(b => b.Order)
                    .ThenInclude(o => o.Table)
                .Include(b => b.Order)
                    .ThenInclude(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                .Include(b => b.Account)
                .FirstOrDefaultAsync(b => b.Id == billId);
        }

        public async Task<Bill?> GetByOrderIdAsync(Guid orderId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(b => b.OrderId == orderId && b.Status != Domain.Enums.BillStatus.cancelled);
        }
    }
}
