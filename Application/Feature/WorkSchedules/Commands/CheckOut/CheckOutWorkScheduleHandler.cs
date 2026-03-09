using Application.Feature.WorkSchedules.Commands.CheckOut;
using MediatR;
using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Interface;
using RestaurantOrderTracking.Domain.Interface.Repository;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Feature.WorkSchedules.Commands.CheckOut
{
    public class CheckOutWorkScheduleHandler : IRequestHandler<CheckOutWorkScheduleCommand, Result<bool>>
    {
        private readonly IWorkScheduleRepository _workScheduleRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CheckOutWorkScheduleHandler(IWorkScheduleRepository workScheduleRepository, IUnitOfWork unitOfWork)
        {
            _workScheduleRepository = workScheduleRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(CheckOutWorkScheduleCommand request, CancellationToken cancellationToken)
        {
            var schedule = await _workScheduleRepository.GetByIdAsync(request.Id, cancellationToken);
            if (schedule == null)
            {
                return Result<bool>.Failure("Work schedule not found");
            }

            if (schedule.ActualCheckIn == null)
            {
                return Result<bool>.Failure("Cannot check out without checking in first");
            }
            
            var workEndDateTime = schedule.WorkDate.ToDateTime(schedule.EndTime);
            if (DateTime.Now > workEndDateTime && schedule.ActualCheckIn == null)
            {
                // This shouldn't be reached since we checked ActualCheckIn above, but keeping consistency.
                schedule.MarkAbsent();
                _workScheduleRepository.Update(schedule, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return Result<bool>.Failure("You missed your shift. Marked as absent.");
            }

            schedule.CheckOut();

            _workScheduleRepository.Update(schedule, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success("Checked out successfully", true);
        }
    }
}
