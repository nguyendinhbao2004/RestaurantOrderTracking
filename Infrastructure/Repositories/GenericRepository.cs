using Microsoft.EntityFrameworkCore;
using RestaurantOrderTracking.Domain.Interface.Repository;
using RestaurantOrderTracking.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace RestaurantOrderTracking.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        // Protected để các Repository con (như CourseRepository) có thể dùng lại
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public virtual async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            // Lưu ý: Chưa gọi SaveChanges() ở đây. UnitOfWork sẽ lo việc đó.
        }

        public virtual void Delete(T entity)
        {
            // Kiểm tra: Nếu object này đang "lạ hoắc" với EF Core (Detached)
            if (_context.Entry(entity).State == EntityState.Detached)
            {
                // Thì phải Attach vào trước để EF Core biết nó là ai
                _dbSet.Attach(entity);
            }
            // Đánh dấu trạng thái là "Đã Xóa" (Deleted)
            _dbSet.Remove(entity);
        }

        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            //Expression<Func<T, bool>> predicate: Đây là biểu thức Lambda
            //Ví dụ: c => c.Price > 100
            //.Where(predicate): Gắn điều kiện lọc vào câu SQL
            return await _dbSet.Where(predicate).AsNoTracking().ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            // AsNoTracking() là chìa khóa tối ưu hiệu năng vì khi lấy data từ sql về thì nó sẽ tự tạo 1 bản sao
            //Lấy dữ liệu từ SQL về và trả cho bạn ngay. Không tạo bản sao, không theo dõi gì cả.
            return await _dbSet.AsNoTracking().ToListAsync();
        }

        public virtual async Task<T?> GetByIdAsync(object id)
        {
            return await _dbSet.FindAsync(id);
        }

        //Khi dữ liệu từ WebAPI (JSON) gửi lên, nó là một object mới tinh trong RAM, EF Core chưa hề biết đến nó (gọi là trạng thái Detached).
        //Attach giúp EF Core nhận diện: "À, đây là dữ liệu thuộc về Database, hãy quản lý nó".
        public virtual void Update(T entity)
        {
            // Attach entity vào DbSet nếu nó đang ở trạng thái Detached
            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
            // EntityState.Modified : Đánh dấu toàn bộ entity là đã thay đổi
            //Dòng này ra lệnh cho EF Core: "Tao không cần biết mày thấy gì, tao tuyên bố object này đã bị thay đổi toàn bộ. Hãy chuẩn bị câu lệnh UPDATE cho tất cả các cột".
        }
    }
}
