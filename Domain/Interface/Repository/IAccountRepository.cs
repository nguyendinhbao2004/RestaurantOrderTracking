using RestaurantOrderTracking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantOrderTracking.Domain.Interface.Repository
{
    public interface IAccountRepository : IGenericRepository<Account>
    {
        Task<(IEnumerable<Account> Accounts, int TotalCount)> GetPageAccountAsync(string? keyword, int pageIndex, int pageSize);
        Task<IEnumerable<Account>> GetAccountsByRoleAsync(int? roleId, CancellationToken cancellationToken = default);
        Task<Account?> GetByUserNameAsync(string userName);
        Task<bool> CheckPasswordAsync(Account account, string password);
        string HashPassword(string password);
    }
}
