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
        public IActionResult GetAllLogs()
        {
            var logs = _logService.GetAllLogs();

            if (logs == null || logs.Count == 0)
            {
                return NotFound("No logs found.");
            }

            return Ok(logs);
        }
    }
}