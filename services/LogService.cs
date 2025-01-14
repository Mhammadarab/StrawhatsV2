using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Cargohub.models;

namespace Cargohub.services
{
    public class LogService
    {
        private readonly string logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "logs", "user_changes.log");

        public List<Dictionary<string, object>> GetAll(string action = null, DateTime? fromDate = null, DateTime? toDate = null, string performedBy = null, string apiKey = null, string changes = null)
        {
            if (!File.Exists(logFilePath))
            {
                Console.WriteLine($"Log file not found at path: {logFilePath}");
                return new List<Dictionary<string, object>>();
            }

            var logs = new List<Dictionary<string, object>>();
            var logLines = File.ReadAllLines(logFilePath);

            foreach (var line in logLines)
            {
                var parsedLog = ParseLogLine(line);
                if (parsedLog != null)
                {
                    logs.Add(parsedLog);
                }
            }

            // Apply filters
            if (!string.IsNullOrEmpty(action))
            {
                logs = logs.Where(log => log["Action"]?.ToString().Equals(action, StringComparison.OrdinalIgnoreCase) == true).ToList();
            }

            if (fromDate.HasValue)
            {
                logs = logs.Where(log => DateTime.Parse(log["Timestamp"].ToString()) >= fromDate.Value).ToList();
            }

            if (toDate.HasValue)
            {
                logs = logs.Where(log => DateTime.Parse(log["Timestamp"].ToString()) <= toDate.Value).ToList();
            }

            if (!string.IsNullOrEmpty(performedBy))
            {
                logs = logs.Where(log => log["PerformedBy"]?.ToString().Equals(performedBy, StringComparison.OrdinalIgnoreCase) == true).ToList();
            }

            if (!string.IsNullOrEmpty(apiKey))
            {
                logs = logs.Where(log => log["ApiKey"]?.ToString().Equals(apiKey, StringComparison.OrdinalIgnoreCase) == true).ToList();
            }

            if (!string.IsNullOrEmpty(changes))
            {
                logs = logs.Where(log => log["Changes"]?.ToString().Contains(changes, StringComparison.OrdinalIgnoreCase) == true).ToList();
            }

            return logs;
        }

        private Dictionary<string, object> ParseLogLine(string line)
        {
            try
            {
                var parts = line.Split('|');
                var logEntry = new Dictionary<string, object>
                {
                    ["Timestamp"] = parts[0].Split('=')[1].Trim(),
                    ["Action"] = parts[1].Split('=')[1].Trim(),
                    ["PerformedBy"] = parts[2].Split('=')[1].Trim(),
                    ["ApiKey"] = parts[3].Split('=')[1].Trim(),
                    ["Changes"] = parts.Length > 4 ? parts[4].Split('=', 2)[1].Trim() : null
                };
                return logEntry;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing log line: {line}. Exception: {ex.Message}");
                return null;
            }
        }
    }
}