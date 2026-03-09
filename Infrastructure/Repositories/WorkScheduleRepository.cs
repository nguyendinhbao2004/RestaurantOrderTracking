using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RestaurantOrderTracking.Domain.Entities;
using RestaurantOrderTracking.Domain.Interface.Repository;
using RestaurantOrderTracking.Infrastructure.Data;

namespace RestaurantOrderTracking.Infrastructure.Repositories
{
    public class WorkScheduleRepository : GenericRepository<WorkSchedule>, IWorkScheduleRepository
    {
        public WorkScheduleRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<(IEnumerable<WorkSchedule>, int totalCount)> GetPagedWorkScheduleAsync(string? keyword, int pageIndex, int pageSize)
        {
            var query = _dbSet.Include(w => w.Account).AsQueryable();
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(w => w.Account.FullName.Contains(keyword) || w.ShiftName.Contains(keyword));
            }   
            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(w => w.WorkDate)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return (items, totalCount);
        }
    }
}
