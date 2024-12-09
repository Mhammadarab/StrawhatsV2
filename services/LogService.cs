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
        private readonly string logFilePath = Path.Combine("Logs", "user_changes.json");

        public List<JObject> GetLogs()
        {
            if (!File.Exists(logFilePath))
            {
                return new List<JObject>();
            }

            var jsonData = File.ReadAllText(logFilePath);
            var logs = JsonConvert.DeserializeObject<List<JObject>>(jsonData) ?? new List<JObject>();
            return logs;
        }

        public List<JObject> FilterLogsByAction(string actionType)
        {
            var logs = GetLogs();
            return logs.Where(log => log["Action"]?.ToString().Equals(actionType, StringComparison.OrdinalIgnoreCase) == true).ToList();
        }

        public List<JObject> FilterLogsByAffectedUser(string userApiKey)
        {
            var logs = GetLogs();
            return logs.Where(log => log["Changes"]?["ApiKey"]?.ToString() == userApiKey).ToList();
        }
    }
}