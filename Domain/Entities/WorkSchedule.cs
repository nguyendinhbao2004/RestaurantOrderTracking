using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantOrderTracking.Domain.Entities
{
    public class WorkSchedule : BaseEntities
    {
        public Guid AccountId { get; private set; }
        public DateOnly WorkDate { get; private set; }
        public TimeOnly StartTime { get; private set; }
        public TimeOnly EndTime { get; private set; }

        public string ShiftName { get; private set; }

        public DateTime? ActualCheckIn { get; private set; }

        public DateTime? ActualCheckOut { get; private set; }
        public WorkScheduleStatus Status { get; set; }
        public string? Note { get; set; }

        public virtual Account Account { get; private set; } = null!;

        public WorkSchedule(Guid accountId, DateOnly workDate, TimeOnly startTime, TimeOnly endTime, string shiftName)
        {
            AccountId = accountId;
            WorkDate = workDate;
            StartTime = startTime;
            EndTime = endTime;
            ShiftName = shiftName;
        }

        public void CheckIn()
        {
            ActualCheckIn = DateTime.UtcNow;
            Status = WorkScheduleStatus.Present;
        }

        public void CheckOut()
        {
            ActualCheckOut = DateTime.UtcNow;
        }

        public void MarkAbsent()
        {
            Status = WorkScheduleStatus.Absent;
        }

        public void UpdateInfo(Guid accountId, DateOnly workDate, TimeOnly startTime, TimeOnly endTime, string shiftName, string? note, WorkScheduleStatus status)
        {
            AccountId = accountId;
            WorkDate = workDate;
            StartTime = startTime;
            EndTime = endTime;
            ShiftName = shiftName;
            Note = note;
            Status = status;
        }
    }
}
