using MediatR;
using RestaurantOrderTracking.Application.Dto.Table;
using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Entities;
using RestaurantOrderTracking.Domain.Interface;
using RestaurantOrderTracking.Domain.Interface.Repository;

namespace RestaurantOrderTracking.Application.Feature.Table.Commands.GenerateQRSession
{
    public class GenerateQRSessionHandler : IRequestHandler<GenerateQRSessionCommand, Result<QRSessionResponse>>
    {
        private readonly ITableRepository _tableRepository;
        private readonly IGenericRepository<QRSession> _qrSessionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public GenerateQRSessionHandler(
            ITableRepository tableRepository,
            IGenericRepository<QRSession> qrSessionRepository,
            IUnitOfWork unitOfWork)
        {
            _tableRepository = tableRepository;
            _qrSessionRepository = qrSessionRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<QRSessionResponse>> Handle(GenerateQRSessionCommand request, CancellationToken cancellationToken)
        {
            // 1. Validate table tồn tại
            var table = await _tableRepository.GetByIdAsync(request.TableId);
            if (table == null)
                return Result<QRSessionResponse>.Failure("Table not found.");

            // 2. Vô hiệu hoá tất cả session cũ của table
            var existingSessions = await _qrSessionRepository.FindAsync(
                s => s.TableId == request.TableId && s.IsActive);
            foreach (var session in existingSessions)
            {
                session.Revoke();
                _qrSessionRepository.Update(session, cancellationToken);
            }

            // 3. Tạo session mới
            var newSession = new QRSession(request.TableId);
            await _qrSessionRepository.AddAsync(newSession);

            // 4. Cập nhật QR code trên Table
            table.UpdateQRCode(newSession.SessionToken);
            _tableRepository.Update(table, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<QRSessionResponse>.Success("QR Session generated successfully.", new QRSessionResponse
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
