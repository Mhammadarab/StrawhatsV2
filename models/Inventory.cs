using System;
using System.Collections.Generic;

namespace Cargohub.models
{
    public class Inventory
    {
        public int Id { get; set; }
        public string Item_Id { get; set; }
        public string Description { get; set; }
        public string Item_Reference { get; set; }
        public Dictionary<int, int> Locations {get;  set;}
        public int Total_On_Hand { get; set; }
        public int Total_Expected { get; set; }
        public int Total_Ordered { get; set; }
        public int Total_Allocated { get; set; }
        public int Total_Available { get; set; }
        public DateTime Created_At { get; set; }
        public DateTime Updated_At { get; set; }
    }
}
