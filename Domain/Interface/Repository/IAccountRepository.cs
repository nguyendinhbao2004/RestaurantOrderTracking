using RestaurantOrderTracking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantOrderTracking.Domain.Interface.Repository
{
    public interface IAccountRepository : IGenericRepository<Account>
    {
        Task<Account?> GetByUserNameAsync(string userName);
        Task<bool> CheckPasswordAsync(Account account, string password);
        string HashPassword(string password);
    }
}
