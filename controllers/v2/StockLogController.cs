using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cargohub.interfaces;
using Cargohub.services;
using Cargohub.models;
using Microsoft.AspNetCore.Mvc;

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
            // Validate pagination parameters if provided
            if ((pageNumber.HasValue && pageNumber <= 0) || (pageSize.HasValue && pageSize <= 0))
            {
                return BadRequest("Page number and page size must be greater than zero if provided.");
            }

            var stockLogs = _stockLogService.GetAll(pageNumber, pageSize);

            if (stockLogs == null || !stockLogs.Any())
            {
                return NotFound();
            }

            var totalRecords = _stockLogService.GetAll(null, null).Count; // Total count without pagination

            // Return metadata only if pagination is applied
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
                return BadRequest("StockLog data is required.");
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

        // PUT: api/v2/stocklogs/{timestamp}/yes
        [HttpPut("{timestamp}/yes")]
        public async Task<IActionResult> ApproveAudit(string timestamp)
        {
            var logEntry = _stockLogService.GetById(timestamp);
            if (logEntry == null)
            {
                return NotFound("Log entry not found.");
            }

            // Convert AuditData from Dictionary<string, Dictionary<string, int>> to Dictionary<int, Dictionary<int, int>>
            var auditData = logEntry.AuditData.ToDictionary(
                kvp => int.Parse(kvp.Key),
                kvp => kvp.Value.ToDictionary(innerKvp => int.Parse(innerKvp.Key), innerKvp => innerKvp.Value)
            );

            // Update the stock based on the audit data
            _inventoryService.AuditInventory(logEntry.PerformedBy, auditData);

            return Ok("Audit approved and inventory updated.");
        }

        // PUT: api/v2/stocklogs/{timestamp}/no
        [HttpPut("{timestamp}/no")]
        public IActionResult RejectAudit(string timestamp)
        {
            var logEntry = _stockLogService.GetById(timestamp);
            if (logEntry == null)
            {
                return NotFound("Log entry not found.");
            }

            // Log the rejection
            logEntry.Discrepancies.Add("Audit rejected by admin.");
            _stockLogService.Update(logEntry);

            return Ok("Audit rejected.");
        }
    }
}