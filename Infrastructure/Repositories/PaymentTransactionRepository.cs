using Microsoft.EntityFrameworkCore;
using RestaurantOrderTracking.Domain.Entities;
using RestaurantOrderTracking.Domain.Interface.Repository;
using RestaurantOrderTracking.Infrastructure.Data;
using System.Threading.Tasks;

namespace RestaurantOrderTracking.Infrastructure.Repositories
{
    public class PaymentTransactionRepository : GenericRepository<PaymentTransaction>, IPaymentTransactionRepository
    {
        public PaymentTransactionRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<PaymentTransaction?> GetByOrderCodeAsync(long orderCode)
        {
            return await _context.PaymentTransactions
                .Include(pt => pt.Bill)
                .FirstOrDefaultAsync(pt => pt.OrderCode == orderCode);
        }
    }
}
