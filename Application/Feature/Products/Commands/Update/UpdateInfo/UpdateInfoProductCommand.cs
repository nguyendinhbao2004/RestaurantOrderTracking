using MediatR;
using RestaurantOrderTracking.Domain.Common;

namespace Application.Feature.Products.Commands.Update.UpdateInfo
{
    public record UpdateInfoProductCommand(Guid Id, string Name, decimal Price, string? Description) : IRequest<Result<Guid>>
    {
        
    }
}