using Microsoft.EntityFrameworkCore;
using RestaurantOrderTracking.Domain.Entities;
using RestaurantOrderTracking.Domain.Interface.Repository;
using RestaurantOrderTracking.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantOrderTracking.Infrastructure.Repositories
{
    public class TableRepository : GenericRepository<Table>, ITableRepository
    {
        public TableRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Table>GetByIdAsync(Guid id)
        {
            var query = _dbSet.Include(t => t.Area)
                                .Include(t => t.Orders)
                                    .ThenInclude(o => o.OrderItems)
                                        .ThenInclude(oi => oi.Product)
                                .AsQueryable();
            query = query.Where(t => t.Id == id);
            var table = await query.FirstOrDefaultAsync();
            return table;
        }

        public async Task<(IEnumerable<Table>, int totalCount)> GetPagedTablesAsync(string? keyword, int pageIndex, int pageSize)
        {
            var query = _dbSet.Include(t => t.Area)
                              .Include(t => t.Orders)
                              .AsQueryable();
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(t => t.TableNumber.Contains(keyword));
            }
            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(t => t.CreatedAt)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return (items, totalCount);
        }

        public async Task<IEnumerable<Table>> GetTablesByAreaIdAsync(Guid areaId)
        {
            var query = _dbSet.Include(t => t.Area)
                                .Include(t => t.Orders)
                                    .ThenInclude(o => o.OrderItems)
                                        .ThenInclude(oi => oi.Product)
                                .AsQueryable();
            query = query.Where(t => t.AreaId == areaId);
            var tables = await query.OrderBy(t => t.TableNumber).ToListAsync();
            return tables;
        }

        public Task<bool> IsOccupedAsync(int tableNumber)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsTableAvailableAsync(int tableNumber)
        {
            throw new NotImplementedException();
        }
    }
}
