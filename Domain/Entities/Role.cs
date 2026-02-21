using System.Collections.Generic;

namespace RestaurantOrderTracking.Domain.Entities
{
    // 1."Admin", "System administrator with full access"
    // 2.Manager", "Restaurant manager with management access"
    // 3."Chef", "Kitchen staff responsible for cooking"
    // 4. "Waiter", "Service staff responsible for serving customers"
    // 5. "Cashier", "Staff responsible for handling payments"
    public class Role
    {
        public int Id { get; private set; }
        public string Name { get; private set; } = null!;
        public string Description { get; private set; } = null!;

        private readonly List<Account> _accounts = new();
        public IReadOnlyCollection<Account> Accounts => _accounts.AsReadOnly();

        protected Role() { }

        public Role(int id, string name, string description)
        {
            Id = id;
            Name = name;
            Description = description;
        }

        public void UpdateRole(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}
