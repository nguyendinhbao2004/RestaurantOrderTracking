using Application.Feature.WorkSchedules.Commands.CheckIn;
using MediatR;
using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Interface;
using RestaurantOrderTracking.Domain.Interface.Repository;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Feature.WorkSchedules.Commands.CheckIn
{
    public class CheckInWorkScheduleHandler : IRequestHandler<CheckInWorkScheduleCommand, Result<bool>>
    {
        private readonly IWorkScheduleRepository _workScheduleRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CheckInWorkScheduleHandler(IWorkScheduleRepository workScheduleRepository, IUnitOfWork unitOfWork)
        {
            _workScheduleRepository = workScheduleRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(CheckInWorkScheduleCommand request, CancellationToken cancellationToken)
        {
            var schedule = await _workScheduleRepository.GetByIdAsync(request.Id, cancellationToken);
            if (schedule == null)
            {
                return Result<bool>.Failure("Work schedule not found");
            }
            
            var workEndDateTime = schedule.WorkDate.ToDateTime(schedule.EndTime);
            if (DateTime.Now > workEndDateTime && schedule.ActualCheckIn == null)
            {
                schedule.MarkAbsent();
                _workScheduleRepository.Update(schedule, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return Result<bool>.Failure("You missed your shift. Marked as absent.");
            }

            schedule.CheckIn();

            _workScheduleRepository.Update(schedule, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success("Checked in successfully", true);
        }
    }
}
