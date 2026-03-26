using RestaurantOrderTracking.Domain.Entities;

namespace RestaurantOrderTracking.Domain.Interface.Repository
{
    public interface IChefRepository : IGenericRepository<Chef>
    {
        Task<IEnumerable<Chef>> GetAvailableChefsAsync();
        Task<Chef?> GetByAccountIdAsync(Guid accountId);
    }
}