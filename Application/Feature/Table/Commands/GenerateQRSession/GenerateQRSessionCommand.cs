using MediatR;
using RestaurantOrderTracking.Application.Dto.Table;
using RestaurantOrderTracking.Domain.Common;

namespace RestaurantOrderTracking.Application.Feature.Table.Commands.GenerateQRSession
{
    public record GenerateQRSessionCommand(Guid TableId) : IRequest<Result<QRSessionResponse>>;
}
