using RestaurantOrderTracking.Domain.Entities;

namespace RestaurantOrderTracking.Domain.Interface.Repository
{
    public interface ICustomerRepository : IGenericRepository<Customer>
    {
        Task<Customer?> GetByPhoneAsync(string phone);
    }
}
