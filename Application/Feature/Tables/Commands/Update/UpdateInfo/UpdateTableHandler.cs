using MediatR;
using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Interface;
using RestaurantOrderTracking.Domain.Interface.Repository;

namespace Application.Feature.Tables.Commands.Update.UpdateInfo
{
    public class UpdateTableHandler : IRequestHandler<UpdateTableCommand, Result<string>>
    {
        private readonly ITableRepository _tableRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateTableHandler(ITableRepository tableRepository, IUnitOfWork unitOfWork)
        {
            _tableRepository = tableRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<Result<string>> Handle(UpdateTableCommand request, CancellationToken cancellationToken)
        {
            var updateTable = await _tableRepository.GetByIdAsync(request.Id, cancellationToken);
            if (updateTable == null)            
                return Result<string>.Failure("Table not found");
            updateTable.UpdateTableInfo(request.areaId, request.tableNumber, request.capacity);
            _tableRepository.Update(updateTable, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<string>.Success("Table updated successfully");
        }
    }
}