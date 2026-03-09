using Application.Feature.WorkSchedules.Commands.Update;
using MediatR;
using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Enums;
using RestaurantOrderTracking.Domain.Interface;
using RestaurantOrderTracking.Domain.Interface.Repository;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Feature.WorkSchedules.Commands.Update
{
    public class UpdateWorkScheduleHandler : IRequestHandler<UpdateWorkScheduleCommand, Result<Guid>>
    {
        private readonly IWorkScheduleRepository _workScheduleRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateWorkScheduleHandler(IWorkScheduleRepository workScheduleRepository, IUnitOfWork unitOfWork)
        {
            _workScheduleRepository = workScheduleRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(UpdateWorkScheduleCommand request, CancellationToken cancellationToken)
        {
            var schedule = await _workScheduleRepository.GetByIdAsync(request.Id, cancellationToken);
            if (schedule == null)
            {
                return Result<Guid>.Failure("Work schedule not found");
            }

            var status = (WorkScheduleStatus)request.Status;

            schedule.UpdateInfo(
                accountId: request.AccountId,
                workDate: request.WorkDate,
                startTime: request.StartTime,
                endTime: request.EndTime,
                shiftName: request.ShiftName,
                note: request.Note,
                status: status
            );

            _workScheduleRepository.Update(schedule, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success("Work schedule updated successfully", schedule.Id);
        }
    }
}
