using System;

namespace Application.Dto.WorkSchedule
{
    public class WorkScheduleResponse
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public string AccountName { get; set; } = string.Empty;
        public DateOnly WorkDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string ShiftName { get; set; } = string.Empty;
        public DateTime? ActualCheckIn { get; set; }
        public DateTime? ActualCheckOut { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Note { get; set; }
    }
}
