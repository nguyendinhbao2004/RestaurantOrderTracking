using MediatR;
using RestaurantOrderTracking.Application.Dto.Table;
using RestaurantOrderTracking.Domain.Common;

namespace RestaurantOrderTracking.Application.Feature.Tables.Queries.GetBySessionToken
{
    /// <summary>
    /// Query to retrieve table information by session token (from QR code scan)
    /// </summary>
    public record GetTableBySessionTokenQuery(string SessionToken) : IRequest<Result<TableInfoBySessionResponse>>;
}
