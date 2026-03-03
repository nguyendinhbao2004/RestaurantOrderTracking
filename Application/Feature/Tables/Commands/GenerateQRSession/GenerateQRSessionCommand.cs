using MediatR;
using RestaurantOrderTracking.Application.Dto.Table;
using RestaurantOrderTracking.Domain.Common;

namespace RestaurantOrderTracking.Application.Feature.Tables.Commands.GenerateQRSession
{
    public record GenerateQRSessionCommand(Guid TableId) : IRequest<Result<QRSessionResponse>>;
}
