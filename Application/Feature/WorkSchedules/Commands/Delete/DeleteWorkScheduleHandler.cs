using Application.Feature.WorkSchedules.Commands.Delete;
using MediatR;
using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Interface;
using RestaurantOrderTracking.Domain.Interface.Repository;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Feature.WorkSchedules.Commands.Delete
{
    public class DeleteWorkScheduleHandler : IRequestHandler<DeleteWorkScheduleCommand, Result<bool>>
    {
        private readonly IWorkScheduleRepository _workScheduleRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteWorkScheduleHandler(IWorkScheduleRepository workScheduleRepository, IUnitOfWork unitOfWork)
        {
            _workScheduleRepository = workScheduleRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(DeleteWorkScheduleCommand request, CancellationToken cancellationToken)
        {
            var schedule = await _workScheduleRepository.GetByIdAsync(request.Id, cancellationToken);
            if (schedule == null)
            {
                return Result<bool>.Failure("Work schedule not found");
            }

            _workScheduleRepository.Delete(schedule, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success("Work schedule deleted successfully", true);
        }
    }
}
