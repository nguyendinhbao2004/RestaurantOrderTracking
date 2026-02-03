using RestaurantOrderTracking.Domain.Entities;
using RestaurantOrderTracking.Domain.Interface.Repository;
using RestaurantOrderTracking.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantOrderTracking.Infrastructure.Repositories
{
    public class TableRepository : GenericRepository<Table>, ITableRepository
    {
        public TableRepository(ApplicationDbContext context) : base(context)
        {
        }

        public Task<bool> IsOccupedAsync(int tableNumber)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsTableAvailableAsync(int tableNumber)
        {
            throw new NotImplementedException();
        }
    }
}
