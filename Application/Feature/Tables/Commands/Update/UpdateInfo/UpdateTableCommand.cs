using MediatR;
using RestaurantOrderTracking.Domain.Common;

namespace Application.Feature.Tables.Commands.Update.UpdateInfo
{
    public record UpdateTableCommand(Guid Id,Guid areaId, string tableNumber, int capacity) : IRequest<Result<string>>
    {
        
    }
}