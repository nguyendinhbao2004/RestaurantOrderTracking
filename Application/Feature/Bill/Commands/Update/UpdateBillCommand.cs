using MediatR;
using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Enums;

namespace RestaurantOrderTracking.Application.Feature.Bill.Commands.Update
{
    public record UpdateBillCommand(
        Guid BillId,
        PaymentMethod? PaymentMethod,
        decimal? Discount
    ) : IRequest<Result>;
}
