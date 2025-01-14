using System;
using System.Collections.Generic;
using Cargohub.services;
using Microsoft.AspNetCore.Mvc;

namespace Cargohub.controllers.v2
{
    [ApiExplorerSettings(GroupName = "Logs")]
    [Route("api/v2/logs")]
    [ApiController]
    public class LogController : ControllerBase
    {
        private readonly LogService _logService;

        public LogController(LogService logService)
        {
            _logService = logService;
        }

        [HttpGet]
        public IActionResult GetLogs([FromQuery] string action = null, [FromQuery] string fromDate = null, [FromQuery] string toDate = null, [FromQuery] string performedBy = null, [FromQuery] string apiKey = null, [FromQuery] string changes = null)
        {
            DateTime? parsedFromDate = null;
            DateTime? parsedToDate = null;

            if (!string.IsNullOrEmpty(fromDate))
            {
                if (!DateTime.TryParseExact(fromDate, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out var tempFromDate))
                {
                    return BadRequest("Invalid fromDate format. Use yyyy-MM-dd.");
                }
                parsedFromDate = tempFromDate;
            }

            if (!string.IsNullOrEmpty(toDate))
            {
                if (!DateTime.TryParseExact(toDate, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out var tempToDate))
                {
                    return BadRequest("Invalid toDate format. Use yyyy-MM-dd.");
                }
                parsedToDate = tempToDate;
            }

            var logs = _logService.GetAll(action, parsedFromDate, parsedToDate, performedBy, apiKey, changes);

            if (logs == null || logs.Count == 0)
            {
                return NotFound("No logs found.");
            }

            return Ok(logs);
        }
    }
}