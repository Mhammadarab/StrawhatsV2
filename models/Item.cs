using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Cargohub.models
{
    public class Item
    {
        [JsonProperty("uid")]
        public string Uid { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("short_description")]
        public string ShortDescription { get; set; }

        [JsonProperty("upc_code")]
        public string UpcCode { get; set; }

        [JsonProperty("model_number")]
        public string ModelNumber { get; set; }

        [JsonProperty("commodity_code")]
        public string CommodityCode { get; set; }

        [JsonProperty("item_line")]
        public int ItemLine { get; set; }

        [JsonProperty("item_group")]
        public int ItemGroup { get; set; }

        [JsonProperty("item_type")]
        public int ItemType { get; set; }

        [JsonProperty("unit_purchase_quantity")]
        public int UnitPurchaseQuantity { get; set; }

        [JsonProperty("unit_order_quantity")]
        public int UnitOrderQuantity { get; set; }

        [JsonProperty("pack_order_quantity")]
        public int PackOrderQuantity { get; set; }

        [JsonProperty("supplier_id")]
        public int SupplierId { get; set; }

        [JsonProperty("supplier_code")]
        public string SupplierCode { get; set; }

        [JsonProperty("supplier_part_number")]
        public string SupplierPartNumber { get; set; }

        [JsonProperty("created_at")]
        public DateTime Created_At { get; set; }

        [JsonProperty("updated_at")]
        public DateTime Updated_At { get; set; }
        // public InventoryTotals InventoryTotals { get; set; }
        public List<int> Classifications_Id {get; set;}

    }

    public class InventoryTotals
    {
        [JsonProperty("total_on_hand")]
        public int TotalOnHand { get; set; }

        [JsonProperty("total_expected")]
        public int TotalExpected { get; set; }

        [JsonProperty("total_ordered")]
        public int TotalOrdered { get; set; }

        [JsonProperty("total_allocated")]
        public int TotalAllocated { get; set; }

        [JsonProperty("total_available")]
        public int TotalAvailable { get; set; }
    }
}