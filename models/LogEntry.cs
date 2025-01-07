namespace Cargohub.models
{
    public class LogEntry
    {
        public string Timestamp { get; set; }
        public string PerformedBy { get; set; }
        public string Status { get; set; }
        public Dictionary<int, Dictionary<int, int>> AuditData { get; set; }
        public List<string> Discrepancies { get; set; }
    }
}