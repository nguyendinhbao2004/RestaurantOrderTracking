using Domain.Interface.Repository;
using Microsoft.EntityFrameworkCore;
using RestaurantOrderTracking.Domain.Entities;
using RestaurantOrderTracking.Infrastructure.Data;
using RestaurantOrderTracking.Infrastructure.Repositories;

namespace Infrastructure.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<(IEnumerable<Product>, int totalCount)> GetPagedProductsAsync(string? keyword, int pageIndex, int pageSize)
        {
            var query = _dbSet.Include(p => p.Category).AsQueryable();
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(p => p.Name.Contains(keyword));
            }
            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return (items, totalCount);
        }
    }
}