using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Cargohub.services
{
    public class LogService
    {
        private readonly string logFilePath = "Logs/user_changes.json";

        public List<JObject> GetAll(int? pageNumber = null, int? pageSize = null)
        {
            if (!File.Exists(logFilePath))
            {
                return new List<JObject>();
            }

            var jsonData = File.ReadAllText(logFilePath);
            var logs = JsonConvert.DeserializeObject<List<JObject>>(jsonData) ?? new List<JObject>();

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

        public List<JObject> FilterLogsByAction(string action)
        {
            var logs = GetAll();
            return logs.Where(log => log["Action"]?.ToString().Equals(action, StringComparison.OrdinalIgnoreCase) == true).ToList();
        }

        public List<JObject> FilterLogsByDate(DateTime date)
        {
            var logs = GetAll();
            return logs.Where(log => DateTime.Parse(log["Timestamp"].ToString()).Date == date.Date).ToList();
        }

        public List<JObject> FilterLogsByPerformedBy(string performedBy)
        {
            var logs = GetAll();
            return logs.Where(log => log["PerformedBy"]?.ToString().Equals(performedBy, StringComparison.OrdinalIgnoreCase) == true).ToList();
        }

        public List<JObject> FilterLogsByApiKey(string apiKey)
        {
            var logs = GetAll();
            return logs.Where(log => log["APIkey"]?.ToString().Equals(apiKey, StringComparison.OrdinalIgnoreCase) == true).ToList();
        }
    }
}