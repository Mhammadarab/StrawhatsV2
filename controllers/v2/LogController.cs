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
            var logs = _logService.GetLogs();
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

        [HttpGet("affecteduser/{userApiKey}")]
        public IActionResult GetLogsByAffectedUser(string userApiKey)
        {
            var logs = _logService.FilterLogsByAffectedUser(userApiKey);
            if (logs == null || logs.Count == 0)
            {
                return NotFound($"No logs found for affected user: {userApiKey}.");
            }
            return Ok(logs);
        }
    }
}