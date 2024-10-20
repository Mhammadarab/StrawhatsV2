using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cargohub.models
{
    public class Order
    {
      public int Id { get; set; }
        public int SourceId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime RequestDate { get; set; }
        public string Reference { get; set; }
        public string ReferenceExtra { get; set; }
        public string OrderStatus { get; set; }
        public string Notes { get; set; }
        public string ShippingNotes { get; set; }
        public string PickingNotes { get; set; }
        public int WarehouseId { get; set; }
        public string? ShipTo { get; set; } // Nullable
        public string? BillTo { get; set; } // Nullable
        public int ShipmentId { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal TotalTax { get; set; }
        public decimal TotalSurcharge { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<Item> Items { get; set; }  
    }
}