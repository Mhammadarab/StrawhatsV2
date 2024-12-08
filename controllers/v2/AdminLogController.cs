using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Cargohub.services;
using Cargohub.interfaces;
using System.Collections.Generic;

namespace Cargohub.Controllers.v2
{
    [ApiExplorerSettings(GroupName = "AdminLogs")]
    [Route("api/v2/adminlogs")]
    [ApiController]
    public class AdminLogController : ControllerBase
    {
        private readonly ICrudService<LogEntry, string> _adminLogService;

        public AdminLogController(ICrudService<LogEntry, string> adminLogService)
        {
            _adminLogService = adminLogService;
        }

        // GET: api/v2/adminlogs
        [HttpGet]
        public IActionResult GetStockLogs([FromQuery] int? pageNumber = null, [FromQuery] int? pageSize = null)
        {
            var logs = _adminLogService.GetAll(pageNumber, pageSize);
            return Ok(logs);
        }

        // GET: api/v2/adminlogs/{timestamp}
        [HttpGet("{timestamp}")]
        public IActionResult GetStockLogByTimestamp(string timestamp)
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
        public async Task<IActionResult> CreateStockLog([FromBody] LogEntry newLogEntry)
        {
            await _adminLogService.Create(newLogEntry);
            return CreatedAtAction(nameof(GetStockLogByTimestamp), new { timestamp = newLogEntry.Timestamp }, newLogEntry);
        }

        // PUT: api/v2/adminlogs/{timestamp}
        [HttpPut("{timestamp}")]
        public async Task<IActionResult> UpdateStockLog(string timestamp, [FromBody] LogEntry updatedLogEntry)
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
        public async Task<IActionResult> DeleteStockLog(string timestamp)
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
    }
}