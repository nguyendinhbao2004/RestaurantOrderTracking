using MediatR;
using RestaurantOrderTracking.Application.Dto.Table;
using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Interface.Repository;

namespace RestaurantOrderTracking.Application.Feature.Tables.Queries.GetBySessionToken
{
    public class GetTableBySessionTokenHandler : IRequestHandler<GetTableBySessionTokenQuery, Result<TableInfoBySessionResponse>>
    {
        private readonly IQRSessionRepository _qrSessionRepository;
        private readonly ITableRepository _tableRepository;

        public GetTableBySessionTokenHandler(
            IQRSessionRepository qrSessionRepository,
            ITableRepository tableRepository)
        {
            _qrSessionRepository = qrSessionRepository;
            _tableRepository = tableRepository;
        }

        public async Task<Result<TableInfoBySessionResponse>> Handle(GetTableBySessionTokenQuery request, CancellationToken cancellationToken)
        {
            // 1. Tìm QRSession theo SessionToken
            var session = await _qrSessionRepository.GetBySessionTokenAsync(request.SessionToken);

            if (session == null)
                return Result<TableInfoBySessionResponse>.Failure("Mã QR không hợp lệ hoặc không tồn tại.");

            // 2. Validate session
            if (!session.IsValid())
            {
                if (session.IsExpired())
                    return Result<TableInfoBySessionResponse>.Failure("Phiên QR đã hết hạn. Vui lòng quét lại mã QR mới.");

                return Result<TableInfoBySessionResponse>.Failure("Phiên QR không còn hoạt động. Vui lòng quét lại mã QR mới.");
            }

            // 3. Lấy thông tin Table
            var table = await _tableRepository.GetByIdAsync(session.TableId);

            if (table == null)
                return Result<TableInfoBySessionResponse>.Failure("Không tìm thấy bàn tương ứng với phiên này.");

            // 4. Tạo response
            var response = new TableInfoBySessionResponse
            {
                TableId = table.Id,
                TableNumber = table.TableNumber,
                AreaName = table.Area?.Name ?? "Không xác định",
                Status = table.Status.ToString(),
                Capacity = table.Capacity,
                SessionToken = session.SessionToken,
                ExpiresAt = session.ExpiresAt
            };

            return Result<TableInfoBySessionResponse>.Success(
                $"Lấy thông tin bàn {table.TableNumber} thành công.",
                response
            );
        }
    }
}
