using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StrawhatsV2.models
{
    public class CrossDockingLogEntry
    {
        public DateTime Timestamp { get; set; }
        public string PerformedBy { get; set; }
        public string Operation { get; set; }
        public Dictionary<string, object> Details { get; set; }
    }
}