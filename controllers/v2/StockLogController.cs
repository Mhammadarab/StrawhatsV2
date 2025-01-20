using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cargohub.interfaces;
using Cargohub.services;
using Cargohub.models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Cargohub.controllers.v2
{
    [ApiExplorerSettings(GroupName = "StockLogs")]
    [Route("api/v2/stocklogs")]
    [ApiController]
    public class StockLogController : ControllerBase
    {
        private readonly ICrudService<LogEntry, string> _stockLogService;
        private readonly InventoryService _inventoryService;

        public StockLogController(ICrudService<LogEntry, string> stockLogService, InventoryService inventoryService)
        {
            _stockLogService = stockLogService;
            _inventoryService = inventoryService;
        }

        // GET: api/v2/stocklogs
        [HttpGet]
        public IActionResult GetStockLogs([FromQuery] int? pageNumber = null, [FromQuery] int? pageSize = null)
        {
            var stockLogs = _stockLogService.GetAll(pageNumber, pageSize);
            var totalRecords = stockLogs.Count;

            if (pageNumber.HasValue && pageSize.HasValue)
            {
                return Ok(new
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalRecords = totalRecords,
                    StockLogs = stockLogs
                });
            }
            return Ok(stockLogs);
        }

        // GET: api/v2/stocklogs/{timestamp}
        [HttpGet("{timestamp}")]
        public IActionResult GetStockLogByTimestamp(string timestamp)
        {
            var logEntry = _stockLogService.GetById(timestamp);
            if (logEntry == null)
            {
                return NotFound("Log entry not found.");
            }
            return Ok(logEntry);
        }

        // POST: api/v2/stocklogs
        [HttpPost]
        public async Task<IActionResult> CreateStockLog([FromBody] LogEntry stockLog)
        {
            if (stockLog == null)
            {
                return BadRequest("Invalid log entry data.");
            }

            await _stockLogService.Create(stockLog);
            return CreatedAtAction(nameof(GetStockLogByTimestamp), new { timestamp = stockLog.Timestamp }, stockLog);
        }

        // PUT: api/v2/stocklogs/{timestamp}
        [HttpPut("{timestamp}")]
        public async Task<IActionResult> UpdateStockLog(string timestamp, [FromBody] LogEntry updatedLogEntry)
        {
            if (updatedLogEntry == null || updatedLogEntry.Timestamp != timestamp)
            {
                return BadRequest("Invalid log entry data.");
            }

            try
            {
                await _stockLogService.Update(updatedLogEntry);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // DELETE: api/v2/stocklogs/{timestamp}
        [HttpDelete("{timestamp}")]
        public async Task<IActionResult> DeleteStockLog(string timestamp)
        {
            try
            {
                await _stockLogService.Delete(timestamp);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("{timestamp}/yes")]
public async Task<IActionResult> ApproveAudit(string timestamp)
{
    var logFilePath = Path.Combine("logs", "inventory_audit.log");
    if (!System.IO.File.Exists(logFilePath))
    {
        return NotFound("Log file not found.");
    }

    var jsonData = await System.IO.File.ReadAllTextAsync(logFilePath);
    var logLines = jsonData.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
    var logs = new List<LogEntry>();

    foreach (var line in logLines)
    {
        var logEntry = ParseLogLine(line);
        if (logEntry != null)
        {
            logs.Add(logEntry);
        }
    }

    var logEntryToApprove = logs.FirstOrDefault(log => log.Timestamp == timestamp);
    if (logEntryToApprove == null)
    {
        return NotFound("Log entry not found.");
    }

    // Update the stock based on the audit data
    var discrepancies = _inventoryService.AuditInventory(logEntryToApprove.PerformedBy, logEntryToApprove.AuditData);

    // Update the status to "Completed"
    logEntryToApprove.Status = "Completed";

    // Write the updated logs back to the file
    var updatedLogLines = logs.Select(log => FormatLogLine(log)).ToArray();
    await System.IO.File.WriteAllLinesAsync(logFilePath, updatedLogLines);

    // Save the updated inventories to the inventories.json file
    var inventories = _inventoryService.GetAll();
    var inventoriesFilePath = Path.Combine("data", "inventories.json");
    await System.IO.File.WriteAllTextAsync(inventoriesFilePath, JsonConvert.SerializeObject(inventories, Formatting.Indented));

    return Ok("Audit approved and inventory updated.");
}

        [HttpPut("{timestamp}/no")]
        public async Task<IActionResult> RejectAudit(string timestamp)
        {
            var logFilePath = Path.Combine("logs", "inventory_audit.log");
            if (!System.IO.File.Exists(logFilePath))
            {
                return NotFound("Log file not found.");
            }

            var jsonData = await System.IO.File.ReadAllTextAsync(logFilePath);
            var logLines = jsonData.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            var logs = new List<LogEntry>();

            foreach (var line in logLines)
            {
                var logEntry = ParseLogLine(line);
                if (logEntry != null)
                {
                    logs.Add(logEntry);
                }
            }

            var logEntryToReject = logs.FirstOrDefault(log => log.Timestamp == timestamp);
            if (logEntryToReject == null)
            {
                return NotFound("Log entry not found.");
            }

            // Update the status to "Rejected"
            logEntryToReject.Status = "Rejected";

            // Write the updated logs back to the file
            var updatedLogLines = logs.Select(log => FormatLogLine(log)).ToArray();
            await System.IO.File.WriteAllLinesAsync(logFilePath, updatedLogLines);

            return Ok("Audit rejected.");
        }

        private LogEntry ParseLogLine(string line)
        {
            var parts = line.Split(" | ");
            if (parts.Length < 5) return null;

            var logEntry = new LogEntry
            {
                Timestamp = parts[0].Split('=')[1],
                PerformedBy = parts[1].Split('=')[1],
                Status = parts[2].Split('=')[1],
                AuditData = JsonConvert.DeserializeObject<Dictionary<int, Dictionary<int, int>>>(parts[3].Split('=')[1]),
                Discrepancies = parts[4].Split('=')[1].Trim('[', ']').Split(", ").ToList()
            };

            return logEntry;
        }

        private string FormatLogLine(LogEntry log)
        {
            return $"Timestamp={log.Timestamp} | PerformedBy={log.PerformedBy} | Status={log.Status} | AuditData={JsonConvert.SerializeObject(log.AuditData)} | Discrepancies=[{string.Join(", ", log.Discrepancies)}]";
        }
    }
}