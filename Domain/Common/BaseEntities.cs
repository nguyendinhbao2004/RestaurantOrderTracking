using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantOrderTracking.Domain.Common
{
    public abstract class BaseEntities
    {
        public Guid Id { get; set; } = new Guid();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; } = false; // Soft Delete
    }
}
