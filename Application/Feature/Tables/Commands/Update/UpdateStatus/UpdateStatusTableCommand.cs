using MediatR;
using RestaurantOrderTracking.Domain.Common;

namespace Application.Feature.Tables.Commands.Update.UpdateStatus
{
    public record UpdateStatusTableCommand(Guid Id, int Status) : IRequest<Result<string>>
    {
        
    }
}