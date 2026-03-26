using Microsoft.EntityFrameworkCore;
using RestaurantOrderTracking.Domain.Entities;
using RestaurantOrderTracking.Domain.Interface.Repository;
using RestaurantOrderTracking.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace RestaurantOrderTracking.Infrastructure.Repositories
{
    public class WaiterRepository : GenericRepository<Waiter>, IWaiterRepository
    {
        public WaiterRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Waiter?> GetByAccountIdAsync(Guid accountId)
        {
            var query = _dbSet.Include(w => w.Account)
                              .Include(w => w.AssignedArea)
                              .AsQueryable();
            
            query = query.Where(w => w.AccountId == accountId);
            var waiter = await query.FirstOrDefaultAsync();
            return waiter;
        }

        public async Task<Waiter?> GetByConditionAsync(Expression<Func<Waiter, bool>> predicate)
        {
            var query = _dbSet.Include(w => w.Account)
                              .Include(w => w.AssignedArea)
                              .AsQueryable();
            
            query = query.Where(predicate);
            var waiter = await query.FirstOrDefaultAsync();
            return waiter;
        }

        public async Task<List<Waiter>> GetWaitersByAreaIdAsync(Guid areaId)
        {
            var query = _dbSet.Include(w => w.Account)
                              .AsQueryable();
            
            query = query.Where(w => w.AssignedAreaId == areaId);
            return await query.ToListAsync();
        }
    }
}
