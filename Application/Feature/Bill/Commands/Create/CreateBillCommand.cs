using MediatR;
using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Enums;

namespace RestaurantOrderTracking.Application.Feature.Bill.Commands.Create
{
    public record CreateBillCommand(
        Guid OrderId,
        Guid CashierAccountId,
        PaymentMethod PaymentMethod,
        decimal? Discount
    ) : IRequest<Result<Guid>>;
}
