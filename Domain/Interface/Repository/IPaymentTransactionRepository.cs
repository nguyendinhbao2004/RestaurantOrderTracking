using RestaurantOrderTracking.Domain.Entities;
using System.Threading.Tasks;

namespace RestaurantOrderTracking.Domain.Interface.Repository
{
    public interface IPaymentTransactionRepository : IGenericRepository<PaymentTransaction>
    {
        Task<PaymentTransaction?> GetByOrderCodeAsync(long orderCode);
    }
}
