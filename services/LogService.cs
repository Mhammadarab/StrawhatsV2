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
        private readonly string logFilePath = "Logs/user_changes.json";

        public List<LogEntry> GetAll(int? pageNumber = null, int? pageSize = null)
        {
            if (!File.Exists(logFilePath))
            {
                return new List<LogEntry>();
            }

            var jsonData = File.ReadAllText(logFilePath);
            var logs = JsonConvert.DeserializeObject<List<LogEntry>>(jsonData) ?? new List<LogEntry>();

            // Apply pagination only if pageNumber and pageSize are provided and valid
            if (pageNumber.HasValue && pageSize.HasValue && pageNumber > 0 && pageSize > 0)
            {
                logs = logs
                    .Skip((pageNumber.Value - 1) * pageSize.Value)
                    .Take(pageSize.Value)
                    .ToList();
            }

            return logs;
        }

        public List<LogEntry> FilterLogsByAction(string action)
        {
            var logs = GetAll();
            return logs.Where(log => log.Action?.Equals(action, StringComparison.OrdinalIgnoreCase) == true).ToList();
        }

        public List<LogEntry> FilterLogsByDate(DateTime date)
        {
            var logs = GetAll();
            return logs.Where(log => log.Timestamp.Date == date.Date).ToList();
        }

        public List<LogEntry> FilterLogsByPerformedBy(string performedBy)
        {
            var logs = GetAll();
            return logs.Where(log => log.PerformedBy?.Equals(performedBy, StringComparison.OrdinalIgnoreCase) == true).ToList();
        }

        public List<LogEntry> FilterLogsByApiKey(string apiKey)
        {
            var logs = GetAll();
            return logs.Where(log => log.ApiKey?.Equals(apiKey, StringComparison.OrdinalIgnoreCase) == true).ToList();
        }
    }
}