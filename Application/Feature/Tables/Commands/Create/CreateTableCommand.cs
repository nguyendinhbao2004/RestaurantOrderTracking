using MediatR;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using RestaurantOrderTracking.Domain.Common;

namespace Application.Feature.Tables.Commands.Create
{
    public record CreateTableCommand(string TableNumber, Guid AreaId, string? QRCode) : IRequest<Result<Guid>>
    {
        
    }
}