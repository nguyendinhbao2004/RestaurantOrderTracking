using MediatR;
using RestaurantOrderTracking.Domain.Common;

namespace RestaurantOrderTracking.Application.Feature.Category.Commands.Update
{
    public record UpdateCategoryCommand(
        int Id,
        string? Name,
        string? Description,
        string? ImageUrl,
        bool? IsActive
    ) : IRequest<Result>;
}
