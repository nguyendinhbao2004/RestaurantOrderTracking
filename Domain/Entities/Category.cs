using System.Collections.Generic;

namespace RestaurantOrderTracking.Domain.Entities
{
    public class Category
    {
        public int Id { get; private set; }
        public string Name { get; private set; } = null!;
        public string? Description { get; private set; }
        public string? ImageUrl { get; private set; }
        public bool IsActive { get; private set; } = true;

        private readonly List<Product> _products = new();
        public IReadOnlyCollection<Product> Products => _products.AsReadOnly();

        protected Category() { }

        public Category(int id, string name, string? description = null, string? imageUrl = null, bool isActive = true)
        {
            Id = id;
            Name = name;
            Description = description;
            ImageUrl = imageUrl;
            IsActive = isActive;
        }

        public void UpdateName(string name)
        {
            Name = name;
        }

        public void UpdateDescription(string? description)
        {
            Description = description;
        }

        public void UpdateImage(string? imageUrl)
        {
            ImageUrl = imageUrl;
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
