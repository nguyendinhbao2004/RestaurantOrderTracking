using RestaurantOrderTracking.Domain.Entities;

namespace RestaurantOrderTracking.Domain.Interface.Repository
{
    public interface IQRSessionRepository : IGenericRepository<QRSession>
    {
        Task<QRSession?> GetBySessionTokenAsync(string sessionToken);
    }
}
