using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Cargohub.interfaces;
using Cargohub.models;
using Newtonsoft.Json;

namespace Cargohub.services
{
    public class AdminLogService : ICrudService<LogEntry, string>
    {
        private readonly string logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "logs", "inventory_audit.json");

        public List<LogEntry> GetAll(int? pageNumber = null, int? pageSize = null)
        {
            if (!File.Exists(logFilePath))
            {
                return new List<LogEntry>();
            }

            var logContent = File.ReadAllText(logFilePath);
            var logs = JsonConvert.DeserializeObject<List<LogEntry>>(logContent) ?? new List<LogEntry>();

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

        public LogEntry GetById(string timestamp)
        {
            var logs = GetAll();
            return logs.FirstOrDefault(log => log.Timestamp == timestamp);
        }

        public async Task Create(LogEntry newLogEntry)
        {
            var logs = GetAll();
            logs.Add(newLogEntry);
            await File.WriteAllTextAsync(logFilePath, JsonConvert.SerializeObject(logs, Formatting.Indented));
        }

        public async Task Update(LogEntry updatedLogEntry)
        {
            var logs = GetAll();
            var logEntryIndex = logs.FindIndex(log => log.Timestamp == updatedLogEntry.Timestamp);

            if (logEntryIndex == -1)
            {
                throw new KeyNotFoundException("Log entry not found.");
            }

            logs[logEntryIndex] = updatedLogEntry;
            await File.WriteAllTextAsync(logFilePath, JsonConvert.SerializeObject(logs, Formatting.Indented));
        }

        public async Task Delete(string timestamp)
        {
            var logs = GetAll();
            var logEntry = logs.FirstOrDefault(log => log.Timestamp == timestamp);

            if (logEntry == null)
            {
                throw new KeyNotFoundException("Log entry not found.");
            }

            logs.Remove(logEntry);
            await File.WriteAllTextAsync(logFilePath, JsonConvert.SerializeObject(logs, Formatting.Indented));
        }
    }
}