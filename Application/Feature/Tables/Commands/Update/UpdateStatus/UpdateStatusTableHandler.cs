using MediatR;
using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Enums;
using RestaurantOrderTracking.Domain.Interface;
using RestaurantOrderTracking.Domain.Interface.Repository;

namespace Application.Feature.Tables.Commands.Update.UpdateStatus
{
    public class UpdateStatusTableHandler : IRequestHandler<UpdateStatusTableCommand, Result<string>>
    {
        private readonly ITableRepository _tableRepository;
        private readonly IUnitOfWork _unitOfWork;
        public UpdateStatusTableHandler(ITableRepository tableRepository, IUnitOfWork unitOfWork)
        {
            _tableRepository = tableRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<Result<string>> Handle(UpdateStatusTableCommand request, CancellationToken cancellationToken)
        {
            var table = await _tableRepository.GetByIdAsync(request.Id, cancellationToken);
            if (table == null)
                return Result<string>.Failure("Table not found");

            if (!Enum.TryParse(request.Status, true, out TableStatus newStatus))
                return Result<string>.Failure("Invalid table status");

            table.UpdateStatus(newStatus);
            _tableRepository.Update(table, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            return Result<string>.Success("Table status updated successfully");
        }
    }
}