using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace RestaurantOrderTracking.Domain.Interface.Repository
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(object id);

        //IEnumerable là một tập hợp các phần tử có thể được lặp qua
        //chỉ cho phép ĐỌC (Read-only), Lazy Loading (Thực thi trễ)
        //Khi bạn viết câu lệnh query, dữ liệu chưa được lấy về ngay. Chỉ khi nào bạn bắt đầu lặp (foreach) hoặc gọi .ToList(), dữ liệu mới thực sự được kéo về hoặc xử lý.
        //Nếu trả về List: Bạn ép người dùng phải nhận một danh sách đã tải hết vào RAM.
        //Nếu trả về IEnumerable: Bạn cho người dùng quyền tự quyết định.Họ có thể lặp qua nó ngay, hoặc lọc tiếp (.Where()) rồi mới lấy về.
        Task<IEnumerable<T>> GetAllAsync();

        //// Expression<Func<T, bool>> chính là biểu thức Lambda: c => c.Price > 100
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

        // Thêm mới (Cần await vì có thể sinh ID hoặc logic bất đồng bộ)
        Task AddAsync(T entity);

        // Cập nhật (EF Core chỉ đánh dấu trạng thái là Modified, không cần async)
        void Update(T entity);

        //Chỉ là đánh dấu trạng thái trong bộ nhớ RAM (Change Tracker) nên chạy cực nhanh, dùng void là chuẩn xác.
        void Delete(T entity);
    }
}
