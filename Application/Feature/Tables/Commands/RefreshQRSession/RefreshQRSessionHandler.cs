using MediatR;
using Microsoft.Extensions.Configuration;
using RestaurantOrderTracking.Application.Dto.Table;
using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Entities;
using RestaurantOrderTracking.Domain.Interface;
using RestaurantOrderTracking.Domain.Interface.Repository;

namespace RestaurantOrderTracking.Application.Feature.Tables.Commands.RefreshQRSession
{
    public class RefreshQRSessionHandler : IRequestHandler<RefreshQRSessionCommand, Result<QRSessionResponse>>
    {
        private readonly ITableRepository _tableRepository;
        private readonly IGenericRepository<QRSession> _qrSessionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IQRCodeService _qrCodeService;
        private readonly string _qrBaseUrl;

        public RefreshQRSessionHandler(
            ITableRepository tableRepository,
            IGenericRepository<QRSession> qrSessionRepository,
            IUnitOfWork unitOfWork,
            IQRCodeService qrCodeService,
            IConfiguration configuration)
        {
            _tableRepository = tableRepository;
            _qrSessionRepository = qrSessionRepository;
            _unitOfWork = unitOfWork;
            _qrCodeService = qrCodeService;
            _qrBaseUrl = configuration["QR:BaseUrl"] ?? "https://localhost:7260/order";
        }

        public async Task<Result<QRSessionResponse>> Handle(RefreshQRSessionCommand request, CancellationToken cancellationToken)
        {
            // 1. Validate table tồn tại
            var table = await _tableRepository.GetByIdAsync(request.TableId);
            if (table == null)
                return Result<QRSessionResponse>.Failure("Table not found.");

            // 2. Tìm session active hiện tại
            var activeSessions = await _qrSessionRepository.FindAsync(
                s => s.TableId == request.TableId && s.IsActive);
            var currentSession = activeSessions.FirstOrDefault();

            if (currentSession != null)
            {
                // 3a. Refresh session hiện tại (sinh token mới, gia hạn thời gian)
                currentSession.Refresh();
                _qrSessionRepository.Update(currentSession, cancellationToken);

                table.UpdateQRCode(currentSession.SessionToken);
                _tableRepository.Update(table, cancellationToken);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                var qrBase64 = _qrCodeService.GenerateBase64($"{_qrBaseUrl}?session={currentSession.SessionToken}");

                return Result<QRSessionResponse>.Success("QR Session refreshed successfully.", new QRSessionResponse
                {
                    TableId = table.Id,
                    TableNumber = table.TableNumber,
                    SessionToken = currentSession.SessionToken,
                    ExpiresAt = currentSession.ExpiresAt,
                    IsActive = currentSession.IsActive,
                    QRCodeBase64 = qrBase64
                });
            }
            else
            {
                // 3b. Không có session active → tạo mới
                var newSession = new QRSession(request.TableId);
                await _qrSessionRepository.AddAsync(newSession);

                table.UpdateQRCode(newSession.SessionToken);
                _tableRepository.Update(table, cancellationToken);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                var qrBase64 = _qrCodeService.GenerateBase64($"{_qrBaseUrl}?session={newSession.SessionToken}");

                return Result<QRSessionResponse>.Success("No active session found. New QR Session created.", new QRSessionResponse
                {
                    TableId = table.Id,
                    TableNumber = table.TableNumber,
                    SessionToken = newSession.SessionToken,
                    ExpiresAt = newSession.ExpiresAt,
                    IsActive = newSession.IsActive,
                    QRCodeBase64 = qrBase64
                });
            }
        }
    }
}
