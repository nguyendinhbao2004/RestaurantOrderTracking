using RestaurantOrderTracking.Domain.Entities;

namespace RestaurantOrderTracking.Domain.Interface.Repository
{
    public interface IOrderItemRepository : IGenericRepository<OrderItem>
    {
        Task<(IEnumerable<OrderItem>, int totalCount)> GetPagedOrderItemsAsync(string? keyword, int pageIndex, int pageSize);
        Task<IEnumerable<OrderItem>> GetOrderItemsByStatusAsync(RestaurantOrderTracking.Domain.Enums.OrderItemStatus status);
        Task<IEnumerable<OrderItem>> GetOrderItemsForAccountAsync(int roleId, Guid accountId);
    }
}
