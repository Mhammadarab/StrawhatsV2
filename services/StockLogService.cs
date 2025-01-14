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
    public class StockLogService : ICrudService<LogEntry, string>
    {
        private readonly string logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "logs", "inventory_audit.log");

        public List<LogEntry> GetAll(int? pageNumber = null, int? pageSize = null)
        {
            if (!File.Exists(logFilePath))
            {
                Console.WriteLine($"Log file not found at path: {logFilePath}");
                return new List<LogEntry>();
            }

            var logEntries = new List<LogEntry>();
            var logLines = File.ReadAllLines(logFilePath);

            foreach (var line in logLines)
            {
                var logEntry = ParseLogLine(line);
                if (logEntry != null)
                {
                    logEntries.Add(logEntry);
                }
            }

            // Apply pagination only if pageNumber and pageSize are provided and valid
            if (pageNumber.HasValue && pageSize.HasValue && pageNumber > 0 && pageSize > 0)
            {
                logEntries = logEntries
                    .Skip((pageNumber.Value - 1) * pageSize.Value)
                    .Take(pageSize.Value)
                    .ToList();
            }

            return logEntries;
        }

        private LogEntry ParseLogLine(string line)
        {
            try
            {
                var parts = line.Split('|');
                var logEntry = new LogEntry
                {
                    Timestamp = parts[0].Split('=')[1].Trim(),
                    PerformedBy = parts[1].Split('=')[1].Trim(),
                    Status = parts[2].Split('=')[1].Trim(),
                    AuditData = JsonConvert.DeserializeObject<Dictionary<int, Dictionary<int, int>>>(parts[3].Split('=')[1].Trim()),
                    Discrepancies = JsonConvert.DeserializeObject<List<string>>(parts[4].Split('=')[1].Trim())
                };
                return logEntry;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing log line: {line}. Exception: {ex.Message}");
                return null;
            }
        }

        public LogEntry GetById(string timestamp)
        {
            var logs = GetAll();
            return logs.FirstOrDefault(log => log.Timestamp == timestamp);
        }

        public async Task Create(LogEntry newLogEntry)
        {
            var logLine = $"Timestamp={newLogEntry.Timestamp:O} | PerformedBy={newLogEntry.PerformedBy} | Status={newLogEntry.Status} | AuditData={JsonConvert.SerializeObject(newLogEntry.AuditData)} | Discrepancies={JsonConvert.SerializeObject(newLogEntry.Discrepancies)}";

            await File.AppendAllTextAsync(logFilePath, logLine + Environment.NewLine);
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
            await File.WriteAllLinesAsync(logFilePath, logs.Select(log => FormatLogEntry(log)));
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
            await File.WriteAllLinesAsync(logFilePath, logs.Select(log => FormatLogEntry(log)));
        }

        private string FormatLogEntry(LogEntry logEntry)
        {
            return $"Timestamp={logEntry.Timestamp:O} | PerformedBy={logEntry.PerformedBy} | Status={logEntry.Status} | AuditData={JsonConvert.SerializeObject(logEntry.AuditData)} | Discrepancies={JsonConvert.SerializeObject(logEntry.Discrepancies)}";
        }
    }
}