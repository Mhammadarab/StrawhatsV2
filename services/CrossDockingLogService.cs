using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using StrawhatsV2.models;

namespace Cargohub.Services
{
    public class CrossDockingLogService
{
    private readonly string _logFilePath = Path.Combine("logs", "cross_docking_logs.json");

    public List<CrossDockingLogEntry> GetAllLogs()
    {
        if (!File.Exists(_logFilePath))
            return new List<CrossDockingLogEntry>();

        var jsonData = File.ReadAllText(_logFilePath);

        // Deserialize into a strongly typed list
        return JsonConvert.DeserializeObject<List<CrossDockingLogEntry>>(jsonData) ?? new List<CrossDockingLogEntry>();
    }

    public List<CrossDockingLogEntry> FilterLogs(DateTime? startDate, DateTime? endDate, string adminApiKey = null)
    {
        var logs = GetAllLogs();

        return logs.Where(log =>
        {
            var matchesDateRange = (!startDate.HasValue || log.Timestamp >= startDate) &&
                                   (!endDate.HasValue || log.Timestamp <= endDate);
            var matchesAdmin = string.IsNullOrEmpty(adminApiKey) || log.PerformedBy == adminApiKey;

            return matchesDateRange && matchesAdmin;
        }).ToList();
    }
}
}