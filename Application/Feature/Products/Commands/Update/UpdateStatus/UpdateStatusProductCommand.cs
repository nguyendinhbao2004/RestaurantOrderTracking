using MediatR;
using RestaurantOrderTracking.Domain.Common;

namespace Application.Feature.Products.Commands.Update.UpdateStatus
{
    public record UpdateStatusProductCommand(Guid Id) : IRequest<Result<Guid>>
    {
        
    }
}