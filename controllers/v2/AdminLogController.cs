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
    [ApiExplorerSettings(GroupName = "AdminLogs")]
    [Route("api/v2/adminlogs")]
    [ApiController]
    public class AdminLogController : ControllerBase
    {
        private readonly ICrudService<LogEntry, string> _adminLogService;
        private readonly InventoryService _inventoryService;

        public AdminLogController(ICrudService<LogEntry, string> adminLogService, InventoryService inventoryService)
        {
            _adminLogService = adminLogService;
            _inventoryService = inventoryService;
        }

        // GET: api/v2/adminlogs
        [HttpGet]
        public IActionResult GetAdminLogs([FromQuery] int? pageNumber = null, [FromQuery] int? pageSize = null)
        {
            // Validate pagination parameters if provided
            if ((pageNumber.HasValue && pageNumber <= 0) || (pageSize.HasValue && pageSize <= 0))
            {
                return BadRequest("Page number and page size must be greater than zero if provided.");
            }

            var adminLogs = _adminLogService.GetAll(pageNumber, pageSize);

            if (adminLogs == null || !adminLogs.Any())
            {
                return NotFound();
            }

            var totalRecords = _adminLogService.GetAll(null, null).Count; // Total count without pagination

            // Return metadata only if pagination is applied
            if (pageNumber.HasValue && pageSize.HasValue)
            {
                return Ok(new
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalRecords = totalRecords,
                    AdminLogs = adminLogs
                });
            }
            return Ok(adminLogs);
        }

        // GET: api/v2/adminlogs/{timestamp}
        [HttpGet("{timestamp}")]
        public IActionResult GetAdminLogByTimestamp(string timestamp)
        {
            var logEntry = _adminLogService.GetById(timestamp);
            if (logEntry == null)
            {
                return NotFound("Log entry not found.");
            }

            return Ok(logEntry);
        }

        // POST: api/v2/adminlogs
        [HttpPost]
        public async Task<IActionResult> CreateAdminLog([FromBody] LogEntry adminLog)
        {
            if (adminLog == null)
            {
                return BadRequest("AdminLog data is required.");
            }

            await _adminLogService.Create(adminLog);
            return CreatedAtAction(nameof(GetAdminLogByTimestamp), new { timestamp = adminLog.Timestamp }, adminLog);
        }

        // PUT: api/v2/adminlogs/{timestamp}
        [HttpPut("{timestamp}")]
        public async Task<IActionResult> UpdateAdminLog(string timestamp, [FromBody] LogEntry updatedLogEntry)
        {
            if (updatedLogEntry == null || updatedLogEntry.Timestamp != timestamp)
            {
                return BadRequest("Invalid log entry data.");
            }

            try
            {
                await _adminLogService.Update(updatedLogEntry);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // DELETE: api/v2/adminlogs/{timestamp}
        [HttpDelete("{timestamp}")]
        public async Task<IActionResult> DeleteAdminLog(string timestamp)
        {
            try
            {
                await _adminLogService.Delete(timestamp);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // PUT: api/v2/adminlogs/{timestamp}/yes
        [HttpPut("{timestamp}/yes")]
        public async Task<IActionResult> ApproveAudit(string timestamp)
        {
            var logEntry = _adminLogService.GetById(timestamp);
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

        // PUT: api/v2/adminlogs/{timestamp}/no
        [HttpPut("{timestamp}/no")]
        public IActionResult RejectAudit(string timestamp)
        {
            var logEntry = _adminLogService.GetById(timestamp);
            if (logEntry == null)
            {
                return NotFound("Log entry not found.");
            }

            // Log the rejection
            logEntry.Discrepancies.Add("Audit rejected by admin.");
            _adminLogService.Update(logEntry);

            return Ok("Audit rejected.");
        }
    }
}