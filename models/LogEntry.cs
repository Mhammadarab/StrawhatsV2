using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cargohub.models
{
    public class LogEntry
    {
        public string Timestamp { get; set; }
        public string PerformedBy { get; set; }
        public string Operation { get; set; }
        public Dictionary<string, Dictionary<string, int>> AuditData { get; set; }
        public List<string> Discrepancies { get; set; }
    }
}