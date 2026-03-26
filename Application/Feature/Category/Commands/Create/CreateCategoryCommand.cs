using MediatR;
using RestaurantOrderTracking.Domain.Common;

namespace RestaurantOrderTracking.Application.Feature.Category.Commands.Create
{
    public record CreateCategoryCommand(
        string Name,
        string? Description,
        string? ImageUrl
    ) : IRequest<Result<int>>;
}
