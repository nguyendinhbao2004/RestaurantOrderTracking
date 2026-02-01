using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantOrderTracking.Domain.Entities
{
    public class Table : BaseEntities
    {
        public string TableNumber { get; private set; } = null!;
        public Guid AreaId { get; private set; }
        public virtual Area Area { get; private set; } = null!;
        public TableStatus Status { get; private set; }
        public string? QRCode { get; set; }

        protected Table() { }

        public Table(string tableNumber, Guid areaId, TableStatus status = TableStatus.Available, string? qrCode = null)
        {
            TableNumber = tableNumber;
            AreaId = areaId;
            Status = status;
            QRCode = qrCode;
        }
        public void UpdateStatus(TableStatus status)
        {
            Status = status;
        }

        public void UpdateQRCode(string qrCode)
        {
            QRCode = qrCode;
        }

        
    }
}
