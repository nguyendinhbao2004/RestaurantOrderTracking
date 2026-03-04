using MediatR;
using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Interface;
using RestaurantOrderTracking.Domain.Interface.Repository;
using AreaEntity = RestaurantOrderTracking.Domain.Entities.Area;

namespace RestaurantOrderTracking.Application.Feature.Area.Commands.Update
{
    public class UpdateAreaHandler : IRequestHandler<UpdateAreaCommand, Result>
    {
        private readonly IGenericRepository<AreaEntity> _areaRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateAreaHandler(IGenericRepository<AreaEntity> areaRepository, IUnitOfWork unitOfWork)
        {
            _areaRepository = areaRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(UpdateAreaCommand request, CancellationToken cancellationToken)
        {
            var area = await _areaRepository.GetByIdAsync(request.Id, cancellationToken);
            if (area == null)
                return Result.Failure("Area not found.");

            if (!string.IsNullOrEmpty(request.Name))
            {
                // Kiểm tra trùng tên (bỏ qua chính nó)
                var existing = await _areaRepository.FindAsync(a => a.Name == request.Name && a.Id != request.Id);
                if (existing.Any())
                    return Result.Failure("Area name already exists.");

                area.UpdateName(request.Name);
            }

            if (request.Description != null)
                area.UpdateDescription(request.Description);

            _areaRepository.Update(area, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success("Area updated successfully.");
        }
    }
}
