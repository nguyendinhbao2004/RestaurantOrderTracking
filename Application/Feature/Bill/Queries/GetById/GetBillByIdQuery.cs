using MediatR;
using RestaurantOrderTracking.Application.Dto.Bill;
using RestaurantOrderTracking.Domain.Common;

namespace RestaurantOrderTracking.Application.Feature.Bill.Queries.GetById
{
    public record GetBillByIdQuery(Guid BillId) : IRequest<Result<BillDetailResponse>>;
}
