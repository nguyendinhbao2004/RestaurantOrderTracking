using MediatR;
using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Interface;
using RestaurantOrderTracking.Domain.Interface.Repository;
using AreaEntity = RestaurantOrderTracking.Domain.Entities.Area;

namespace RestaurantOrderTracking.Application.Feature.Area.Commands.Delete
{
    public class DeleteAreaHandler : IRequestHandler<DeleteAreaCommand, Result>
    {
        private readonly IGenericRepository<AreaEntity> _areaRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteAreaHandler(IGenericRepository<AreaEntity> areaRepository, IUnitOfWork unitOfWork)
        {
            _areaRepository = areaRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(DeleteAreaCommand request, CancellationToken cancellationToken)
        {
            var area = await _areaRepository.GetByIdAsync(request.Id, cancellationToken);
            if (area == null)
                return Result.Failure("Area not found.");

            // Kiểm tra còn bàn thuộc khu vực này không
            if (area.Tables.Any())
                return Result.Failure("Cannot delete area that has tables. Please remove or reassign tables first.");

            // Kiểm tra còn waiter phục vụ khu vực này không
            if (area.Waiters.Any())
                return Result.Failure("Cannot delete area that has waiters assigned. Please reassign waiters first.");

            _areaRepository.Delete(area, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success("Area deleted successfully.");
        }
    }
}
