using MediatR;
using RestaurantOrderTracking.Domain.Common;
using System;

namespace Application.Feature.WorkSchedules.Commands.Update
{
    public class UpdateWorkScheduleCommand : IRequest<Result<Guid>>
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public DateOnly WorkDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string ShiftName { get; set; } = string.Empty;
        public string? Note { get; set; }
        public int Status { get; set; }
    }
}
