using RestaurantOrderTracking.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantOrderTracking.Domain.Entities
{
    public class Product : BaseEntities
    {
        public int CategoryId { get; private set; }
        public virtual Category Categories { get; private set; } = null!;

        public string Name { get; private set; } = null!;

        public decimal Price { get; private set; }

        public Boolean IsActive { get; set; }   
        
        protected Product() { }

        public Product(int categoryId, string name, decimal price, bool isActive = true)
        {
            CategoryId = categoryId;
            Name = name;
            Price = price;
            IsActive = isActive;
        }

        public void UpdateInfo(string name, decimal price, bool isActive)
        {
            Name = name;
            Price = price;
            IsActive = isActive;
        }



    }
}
