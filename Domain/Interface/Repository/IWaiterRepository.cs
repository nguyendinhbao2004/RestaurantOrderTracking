using RestaurantOrderTracking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace RestaurantOrderTracking.Domain.Interface.Repository
{
    public interface IWaiterRepository : IGenericRepository<Waiter>
    {
        Task<Waiter?> GetByAccountIdAsync(Guid accountId);
        Task<Waiter?> GetByConditionAsync(Expression<Func<Waiter, bool>> predicate);
        Task<List<Waiter>> GetWaitersByAreaIdAsync(Guid areaId);
    }
}
