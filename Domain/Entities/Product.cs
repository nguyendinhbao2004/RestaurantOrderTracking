using RestaurantOrderTracking.Domain.Common;
using System;
using System.Collections.Generic;

namespace RestaurantOrderTracking.Domain.Entities
{
    public class Product : BaseEntities
    {
        public int CategoryId { get; private set; }
        public virtual Category Category { get; private set; } = null!;

        public string Name { get; private set; } = null!;
        public string? Description { get; private set; }
        public string? ImageUrl { get; private set; }
        public decimal Price { get; private set; }
        public bool IsActive { get; private set; }

        private readonly List<OrderItem> _orderItems = new();
        public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();

        protected Product() { }

        public Product(int categoryId, string name, decimal price, bool isActive = true, string? description = null, string? imageUrl = null)
        {
            CategoryId = categoryId;
            Name = name;
            Price = price;
            IsActive = isActive;
            Description = description;
            ImageUrl = imageUrl;
        }

        public void UpdateInfo(string name, decimal price, bool isActive)
        {
            Name = name;
            Price = price;
            IsActive = isActive;
        }

        public void UpdateDescription(string? description)
        {
            Description = description;
        }

        public void UpdateImage(string? imageUrl)
        {
            ImageUrl = imageUrl;
        }

        public void UpdateCategory(int categoryId)
        {
            CategoryId = categoryId;
        }

        public void Activate()
        {
            IsActive = true;
        }

        public void Deactivate()
        {
            IsActive = false;
        }
    }
}
