using Application.Feature.WorkSchedules.Commands.Create;
using MediatR;
using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Entities;
using RestaurantOrderTracking.Domain.Interface;
using RestaurantOrderTracking.Domain.Interface.Repository;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Feature.WorkSchedules.Commands.Create
{
    public class CreateWorkScheduleHandler : IRequestHandler<CreateWorkScheduleCommand, Result<Guid>>
    {
        private readonly IWorkScheduleRepository _workScheduleRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateWorkScheduleHandler(IWorkScheduleRepository workScheduleRepository, IUnitOfWork unitOfWork)
        {
            _workScheduleRepository = workScheduleRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(CreateWorkScheduleCommand request, CancellationToken cancellationToken)
        {
            var schedule = new WorkSchedule(
                accountId: request.AccountId,
                workDate: request.WorkDate,
                startTime: request.StartTime,
                endTime: request.EndTime,
                shiftName: request.ShiftName
            );

            if (!string.IsNullOrEmpty(request.Note))
            {
                schedule.Note = request.Note;
            }

            await _workScheduleRepository.AddAsync(schedule);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success("Work schedule created successfully", schedule.Id);
        }
    }
}
