using MediatR;
using RestaurantOrderTracking.Domain.Common;

namespace Application.Feature.Tables.Commands.Update.UpdateStatus
{
    public record UpdateStatusTableCommand(Guid Id, string Status) : IRequest<Result<string>>
    {
        
    }
}