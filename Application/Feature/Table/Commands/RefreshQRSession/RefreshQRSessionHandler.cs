using MediatR;
using RestaurantOrderTracking.Application.Dto.Table;
using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Entities;
using RestaurantOrderTracking.Domain.Interface;
using RestaurantOrderTracking.Domain.Interface.Repository;

namespace RestaurantOrderTracking.Application.Feature.Table.Commands.RefreshQRSession
{
    public class RefreshQRSessionHandler : IRequestHandler<RefreshQRSessionCommand, Result<QRSessionResponse>>
    {
        private readonly ITableRepository _tableRepository;
        private readonly IGenericRepository<QRSession> _qrSessionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RefreshQRSessionHandler(
            ITableRepository tableRepository,
            IGenericRepository<QRSession> qrSessionRepository,
            IUnitOfWork unitOfWork)
        {
            _tableRepository = tableRepository;
            _qrSessionRepository = qrSessionRepository;
            _unitOfWork = unitOfWork;
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
                // 3a. Refresh session hiện tại
                currentSession.Refresh();
                _qrSessionRepository.Update(currentSession, cancellationToken);

                // Cập nhật QR code trên Table
                table.UpdateQRCode(currentSession.SessionToken);
                _tableRepository.Update(table, cancellationToken);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result<QRSessionResponse>.Success("QR Session refreshed successfully.", new QRSessionResponse
                {
                    TableId = table.Id,
                    TableNumber = table.TableNumber,
                    SessionToken = currentSession.SessionToken,
                    ExpiresAt = currentSession.ExpiresAt,
                    IsActive = currentSession.IsActive
                });
            }
            else
            {
                // 3b. Nếu không có session active → tạo mới
                var newSession = new QRSession(request.TableId);
                await _qrSessionRepository.AddAsync(newSession);

                table.UpdateQRCode(newSession.SessionToken);
                _tableRepository.Update(table, cancellationToken);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result<QRSessionResponse>.Success("No active session found. New QR Session created.", new QRSessionResponse
                {
                    TableId = table.Id,
                    TableNumber = table.TableNumber,
                    SessionToken = newSession.SessionToken,
                    ExpiresAt = newSession.ExpiresAt,
                    IsActive = newSession.IsActive
                });
            }
        }
    }
}
