using Microsoft.EntityFrameworkCore;
using RestaurantOrderTracking.Domain.Entities;
using RestaurantOrderTracking.Domain.Interface.Repository;
using RestaurantOrderTracking.Infrastructure.Data;

namespace RestaurantOrderTracking.Infrastructure.Repositories
{
    public class QRSessionRepository : GenericRepository<QRSession>, IQRSessionRepository
    {
        public QRSessionRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<QRSession?> GetBySessionTokenAsync(string sessionToken)
        {
            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.SessionToken == sessionToken);
        }
    }
}
