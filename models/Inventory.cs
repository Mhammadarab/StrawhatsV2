using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cargohub.models
{
    public class Inventory
    {
        public int Id { get; set; }
        public string ItemId { get; set; }
        public string Description { get; set; }
        public string ItemReference { get; set; }
        public List<int> Locations { get; set; }
        public int TotalOnHand { get; set; }
        public int TotalExpected { get; set; }
        public int TotalOrdered { get; set; }
        public int TotalAllocated { get; set; }
        public int TotalAvailable { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}