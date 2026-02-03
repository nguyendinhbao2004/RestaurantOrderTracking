using Microsoft.AspNetCore.Identity;
using RestaurantOrderTracking.Domain.Entities;
using RestaurantOrderTracking.Domain.Interface.Repository;
using RestaurantOrderTracking.Infrastructure.Data;
using BCrypt.Net;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace RestaurantOrderTracking.Infrastructure.Repositories
{
    public class AccountRepository : GenericRepository<Account>, IAccountRepository
    {
        private readonly PasswordHasher<Account> _passwordHasher = new PasswordHasher<Account>();

        public AccountRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<bool> CheckPasswordAsync(Account account, string password)
        {
            if (account == null || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(account.PasswordHash))
            {
                return false;
            }

            // BCrypt tự xử lý việc so sánh với Salt bên trong PasswordHash
            // Không dùng toán tử == vì PasswordHash mỗi lần băm sẽ khác nhau
            bool isValid = BCrypt.Net.BCrypt.Verify(password, account.PasswordHash);

            return await Task.FromResult(isValid);
        }

        public async Task<Account?> GetByUserNameAsync(string userName)
        {
            return await _dbSet.Include(a => a.Role).FirstOrDefaultAsync(a => a.UserName == userName);
        }
        public string HashPassword(string password)
        {
            // BCrypt sẽ tự động tạo Salt và băm mật khẩu
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}
