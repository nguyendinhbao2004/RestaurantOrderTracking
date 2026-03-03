using MediatR;
using RestaurantOrderTracking.Application.Dto.Table;
using RestaurantOrderTracking.Domain.Common;

namespace RestaurantOrderTracking.Application.Feature.Tables.Commands.RefreshQRSession
{
    public record RefreshQRSessionCommand(Guid TableId) : IRequest<Result<QRSessionResponse>>;
}
