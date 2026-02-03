using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Enums;
using System;
using System.Collections.Generic;

namespace RestaurantOrderTracking.Domain.Entities
{
    public class Table : BaseEntities
    {
        public string TableNumber { get; private set; } = null!;
        public Guid AreaId { get; private set; }
        public virtual Area Area { get; private set; } = null!;
        public TableStatus Status { get; private set; }
        public string? QRCode { get; private set; }
        public int Capacity { get; private set; }

        private readonly List<Notification> _notifications = new();
        public IReadOnlyCollection<Notification> Notifications => _notifications.AsReadOnly();

        private readonly List<Order> _orders = new();
        public IReadOnlyCollection<Order> Orders => _orders.AsReadOnly();

        protected Table() { }

        public Table(string tableNumber, Guid areaId, int capacity = 4, TableStatus status = TableStatus.Available, string? qrCode = null)
        {
            Id = Guid.NewGuid();
            TableNumber = tableNumber;
            AreaId = areaId;
            Capacity = capacity;
            Status = status;
            QRCode = qrCode;
            CreatedAt = DateTime.UtcNow;
        }

        public void UpdateStatus(TableStatus status)
        {
            Status = status;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateQRCode(string qrCode)
        {
            QRCode = qrCode;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateTableInfo(string tableNumber, int capacity)
        {
            TableNumber = tableNumber;
            Capacity = capacity;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetAvailable()
        {
            Status = TableStatus.Available;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetOccupied()
        {
            Status = TableStatus.Occupied;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetReserved()
        {
            Status = TableStatus.Reserved;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
