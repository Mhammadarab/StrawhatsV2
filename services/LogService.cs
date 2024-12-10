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

        public List<LoggingEntry> GetAll(int? pageNumber = null, int? pageSize = null)
        {
            if (!File.Exists(logFilePath))
            {
                return new List<LoggingEntry>();
            }

            var jsonData = File.ReadAllText(logFilePath);
            var logs = JsonConvert.DeserializeObject<List<LoggingEntry>>(jsonData) ?? new List<LoggingEntry>();

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

        public List<LoggingEntry> FilterLogsByAction(string action)
        {
            var logs = GetAll();
            return logs.Where(log => log.Action?.Equals(action, StringComparison.OrdinalIgnoreCase) == true).ToList();
        }

        public List<LoggingEntry> FilterLogsByDate(DateTime date)
        {
            var logs = GetAll();
            return logs.Where(log => log.Timestamp.Date == date.Date).ToList();
        }

        public List<LoggingEntry> FilterLogsByPerformedBy(string performedBy)
        {
            var logs = GetAll();
            return logs.Where(log => log.PerformedBy?.Equals(performedBy, StringComparison.OrdinalIgnoreCase) == true).ToList();
        }

        public List<LoggingEntry> FilterLogsByApiKey(string apiKey)
        {
            var logs = GetAll();
            return logs.Where(log => log.ApiKey?.Equals(apiKey, StringComparison.OrdinalIgnoreCase) == true).ToList();
        }
    }
}