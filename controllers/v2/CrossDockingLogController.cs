using System;
using Microsoft.AspNetCore.Mvc;
using Cargohub.Services;

namespace Cargohub.Controllers.v2
{
    [ApiExplorerSettings(GroupName = "CrossDockingLogs")]
    [Route("api/v2/cross-docking-logs")]
    [ApiController]
    public class CrossDockingLogController : ControllerBase
    {
        private readonly CrossDockingLogService _logService;

        public CrossDockingLogController(CrossDockingLogService logService)
        {
            _logService = logService;
        }

        /// <summary>
        /// Get all cross-docking logs.
        /// </summary>
        /// <returns>List of all logs.</returns>
        [HttpGet]
        public IActionResult GetAllLogs()
        {
            try
            {
                var logs = _logService.GetAllLogs();
                return Ok(logs);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Filter cross-docking logs by date range or admin API key.
        /// </summary>
        /// <param name="startDate">Start date for filtering (optional).</param>
        /// <param name="endDate">End date for filtering (optional).</param>
        /// <param name="adminApiKey">Admin API key for filtering (optional).</param>
        /// <returns>Filtered list of logs.</returns>
        [HttpGet("filter")]
        public IActionResult FilterLogs([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, [FromQuery] string ApiKey)
        {
            try
            {
                var logs = _logService.FilterLogs(startDate, endDate, ApiKey);
                return Ok(logs);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}