using RestaurantOrderTracking.Domain.Entities;
using RestaurantOrderTracking.Domain.Interface.Repository;
using RestaurantOrderTracking.Infrastructure.Data;

namespace RestaurantOrderTracking.Infrastructure.Repositories
{
    public class OrderItemLogRepository : GenericRepository<OrderItemLog>, IOrderItemLogRepository
    {
        public OrderItemLogRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
