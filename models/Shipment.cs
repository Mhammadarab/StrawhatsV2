using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cargohub.models
{
    public class Shipment
    {
        public int Id { get; set; }
        public List<int> Order_Id { get; set; }
        public int Source_Id { get; set; }
        public DateTime Order_Date { get; set; }
        public DateTime Request_Date { get; set; }
        public DateTime Shipment_Date { get; set; }
        public string Shipment_Type { get; set; }
        public string Shipment_Status { get; set; }
        public string Notes { get; set; }
        public string Carrier_Code { get; set; }
        public string Carrier_Description { get; set; }
        public string Service_Code { get; set; }
        public string Payment_Type { get; set; }
        public string Transfer_Mode { get; set; }
        public int Total_Package_Count { get; set; }
        public double Total_Package_Weight { get; set; }
        public DateTime Created_At { get; set; }
        public DateTime Updated_At { get; set; }
        public List<ItemDetail> Items { get; set; }
    }
}