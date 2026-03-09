using RestaurantOrderTracking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantOrderTracking.Domain.Interface.Repository
{
    public interface IWorkScheduleRepository : IGenericRepository<WorkSchedule>
    {
        Task<(IEnumerable<WorkSchedule>, int totalCount)> GetPagedWorkScheduleAsync(string? keyword, int pageIndex, int pageSize);
    }
}
