using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using StrawhatsV2.models;

namespace Cargohub.Services
{
    public class CrossDockingLogService
    {
        private readonly string _logFilePath = Path.Combine("logs", "cross_docking_logs.log");

        public List<CrossDockingLogEntry> GetAllLogs()
        {
            if (!File.Exists(_logFilePath))
                return new List<CrossDockingLogEntry>();

            var logEntries = new List<CrossDockingLogEntry>();
            var logLines = File.ReadAllLines(_logFilePath);

            foreach (var line in logLines)
            {
                var logEntry = ParseLogLine(line);
                if (logEntry != null)
                {
                    logEntries.Add(logEntry);
                }
            }

            return logEntries;
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

        private CrossDockingLogEntry ParseLogLine(string line)
        {
            try
            {
                var parts = line.Split('|');
                var logEntry = new CrossDockingLogEntry
                {
                    Timestamp = DateTime.Parse(parts[0].Split('=')[1].Trim()),
                    PerformedBy = parts[1].Split('=')[1].Trim(),
                    Operation = parts[2].Split('=')[1].Trim(),
                    Details = JsonConvert.DeserializeObject<Dictionary<string, object>>(parts[3].Split('=')[1].Trim())
                };
                return logEntry;
            }
            catch
            {
                // Handle parsing errors if necessary
                return null;
            }
        }
    }
}