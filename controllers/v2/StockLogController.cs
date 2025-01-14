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
    }
}