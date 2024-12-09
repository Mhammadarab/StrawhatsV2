using System;
using System.Collections.Generic;
using Cargohub.services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

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
        public IActionResult GetLogs()
        {
            var logs = _logService.GetAll();
            if (logs == null || logs.Count == 0)
            {
                return NotFound("No logs found.");
            }
            return Ok(logs);
        }

        [HttpGet("created")]
        public IActionResult GetCreatedLogs()
        {
            var logs = _logService.FilterLogsByAction("Created");
            if (logs == null || logs.Count == 0)
            {
                return NotFound("No created logs found.");
            }
            return Ok(logs);
        }

        [HttpGet("updated")]
        public IActionResult GetUpdatedLogs()
        {
            var logs = _logService.FilterLogsByAction("Updated");
            if (logs == null || logs.Count == 0)
            {
                return NotFound("No updated logs found.");
            }
            return Ok(logs);
        }

        [HttpGet("deleted")]
        public IActionResult GetDeletedLogs()
        {
            var logs = _logService.FilterLogsByAction("Deleted");
            if (logs == null || logs.Count == 0)
            {
                return NotFound("No deleted logs found.");
            }
            return Ok(logs);
        }

        [HttpGet("date/{date}")]
        public IActionResult GetLogsByDate(string date)
        {
            if (!DateTime.TryParseExact(date, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out var parsedDate))
            {
                return BadRequest("Invalid date format. Use dd-MM-yyyy.");
            }

            var logs = _logService.FilterLogsByDate(parsedDate);
            if (logs == null || logs.Count == 0)
            {
                return NotFound($"No logs found for date: {date}.");
            }
            return Ok(logs);
        }

        [HttpGet("performedby/{performedBy}")]
        public IActionResult GetLogsByPerformedBy(string performedBy)
        {
            var logs = _logService.FilterLogsByPerformedBy(performedBy);
            if (logs == null || logs.Count == 0)
            {
                return NotFound($"No logs found for performedBy: {performedBy}.");
            }
            return Ok(logs);
        }

        [HttpGet("APIkey/{apiKey}")]
        public IActionResult GetLogsByApiKey(string apiKey)
        {
            var logs = _logService.FilterLogsByApiKey(apiKey);
            if (logs == null || logs.Count == 0)
            {
                return NotFound($"No logs found for API key: {apiKey}.");
            }
            return Ok(logs);
        }
    }
}