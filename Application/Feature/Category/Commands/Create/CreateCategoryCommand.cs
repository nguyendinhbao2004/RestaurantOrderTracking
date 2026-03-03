using MediatR;
using RestaurantOrderTracking.Domain.Common;

namespace RestaurantOrderTracking.Application.Feature.Category.Commands.Create
{
    public record CreateCategoryCommand(
        int Id,
        string Name,
        string? Description,
        string? ImageUrl
    ) : IRequest<Result<int>>;
}
