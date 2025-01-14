using System;
using System.Collections.Generic;
using System.IO;

namespace Cargohub.services
{
    public class LogService
    {
        private readonly string logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "logs", "user_changes.log");

        public List<Dictionary<string, object>> GetAllLogs()
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