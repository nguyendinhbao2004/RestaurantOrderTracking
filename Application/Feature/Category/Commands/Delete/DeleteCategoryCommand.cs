using MediatR;
using RestaurantOrderTracking.Domain.Common;

namespace RestaurantOrderTracking.Application.Feature.Category.Commands.Delete
{
    public record DeleteCategoryCommand(int Id) : IRequest<Result>;
}
