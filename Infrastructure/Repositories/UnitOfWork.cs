using RestaurantOrderTracking.Domain.Interface;
using RestaurantOrderTracking.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantOrderTracking.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public void Dispose()
        {
            //// Giải phóng tài nguyên kết nối khi xong việc
            _context.Dispose();
        }
    }
}
