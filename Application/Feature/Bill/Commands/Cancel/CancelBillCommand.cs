using MediatR;
using RestaurantOrderTracking.Domain.Common;

namespace RestaurantOrderTracking.Application.Feature.Bill.Commands.Cancel
{
    public record CancelBillCommand(Guid BillId) : IRequest<Result>;
}
