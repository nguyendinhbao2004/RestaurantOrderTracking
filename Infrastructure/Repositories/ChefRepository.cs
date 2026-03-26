using Microsoft.EntityFrameworkCore;
using Domain.Enums;
using RestaurantOrderTracking.Domain.Entities;
using RestaurantOrderTracking.Domain.Interface.Repository;
using RestaurantOrderTracking.Infrastructure.Data;

namespace RestaurantOrderTracking.Infrastructure.Repositories
{
    public class ChefRepository : GenericRepository<Chef>, IChefRepository
    {
        public ChefRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Chef>> GetAvailableChefsAsync()
        {
            var availableChefs = await _dbSet
                .Include(c => c.Account)
                .Where(c => c.IsAvailable)
                .OrderBy(c => c.SkillLevel)
                .ToListAsync();

            return availableChefs
                .Where(c => c.Specialty != ExpertiseChef.HeadChef)
                .ToList();
        }

        public async Task<Chef?> GetByAccountIdAsync(Guid accountId)
        {
            return await _dbSet.FirstOrDefaultAsync(c => c.AccountId == accountId);
        }
    }
}