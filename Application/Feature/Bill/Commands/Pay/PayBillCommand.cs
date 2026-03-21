using MediatR;
using RestaurantOrderTracking.Domain.Common;

namespace RestaurantOrderTracking.Application.Feature.Bill.Commands.Pay
{
    public record PayBillCommand(
        Guid BillId
    ) : IRequest<Result>;
}
