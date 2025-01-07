using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StrawhatsV2.models
{
    public class PickingLogEntry
    {
        public DateTime Timestamp { get; set; }
        public string PerformedBy { get; set; }
        public int ShipmentId { get; set; }
        public Dictionary<string, int> PickedItems { get; set; }
        public string Description { get; set; }
    }
}