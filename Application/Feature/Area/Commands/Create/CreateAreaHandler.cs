using MediatR;
using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Interface;
using RestaurantOrderTracking.Domain.Interface.Repository;
using System;
using AreaEntity = RestaurantOrderTracking.Domain.Entities.Area;

namespace RestaurantOrderTracking.Application.Feature.Area.Commands.Create
{
    public class CreateAreaHandler : IRequestHandler<CreateAreaCommand, Result<Guid>>
    {
        private readonly IGenericRepository<AreaEntity> _areaRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateAreaHandler(IGenericRepository<AreaEntity> areaRepository, IUnitOfWork unitOfWork)
        {
            _areaRepository = areaRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(CreateAreaCommand request, CancellationToken cancellationToken)
        {
            // Kiểm tra trùng tên
            var existing = await _areaRepository.FindAsync(a => a.Name == request.Name);
            if (existing.Any())
                return Result<Guid>.Failure("Area name already exists.");

            var area = new AreaEntity(
                name: request.Name,
                description: request.Description
            );

            await _areaRepository.AddAsync(area);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success("Area created successfully.", area.Id);
        }
    }
}
