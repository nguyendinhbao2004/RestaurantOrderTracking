using RestaurantOrderTracking.Domain.Entities;
using RestaurantOrderTracking.Domain.Interface.Repository;

namespace Domain.Interface.Repository
{
    public interface IProductRepository : IGenericRepository<Product>
    {
         Task<(IEnumerable<Product>, int totalCount)> GetPagedProductsAsync(string? keyword, int pageIndex, int pageSize);
    }
}