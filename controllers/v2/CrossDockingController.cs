using Microsoft.AspNetCore.Mvc;

namespace Cargohub.Controllers.v2
{
    [ApiExplorerSettings(GroupName = "CrossDocking")]
    [Route("api/v2/cross-docking")]
    [ApiController]
    public class CrossDockingController : ControllerBase
    {
        private readonly CrossDockingService _crossDockingService;

        public CrossDockingController(CrossDockingService crossDockingService)
        {
            _crossDockingService = crossDockingService;
        }

        /// <summary>
        /// Matches items between shipments and orders, with optional filtering by shipment ID and pagination.
        /// </summary>
        /// <param name="shipmentId">Optional shipment ID to filter matches.</param>
        /// <param name="pageNumber">Page number for pagination.</param>
        /// <param name="pageSize">Page size for pagination.</param>
        /// <returns>A list of matched items.</returns>
        [HttpGet("match")]
        public IActionResult MatchItems([FromQuery] int? shipmentId = null, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var matches = _crossDockingService.MatchItems(shipmentId, pageNumber, pageSize);
                return Ok(matches);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Marks a shipment as "In Transit."
        /// </summary>
        /// <param name="shipmentId">ID of the shipment to receive.</param>
        /// <returns>A success message if the shipment is received.</returns>
        [HttpPost("receive")]
        public IActionResult ReceiveShipment([FromBody] int shipmentId)
        {
            try
            {
                // Extract API key from headers
                var apiKey = Request.Headers["API_KEY"].FirstOrDefault();
                if (string.IsNullOrEmpty(apiKey))
                {
                    return Unauthorized("API_KEY header is required.");
                }

                var message = _crossDockingService.ReceiveShipment(shipmentId, apiKey);
                return Ok(new { message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Marks a shipment and its items as "Shipped."
        /// </summary>
        /// <param name="shipmentId">ID of the shipment to ship.</param>
        /// <returns>A success message if the shipment is shipped.</returns>
        [HttpPost("ship")]
        public IActionResult ShipShipment([FromBody] int shipmentId)
        {
            try
            {
                // Extract API key from headers
                var apiKey = Request.Headers["API_KEY"].FirstOrDefault();
                if (string.IsNullOrEmpty(apiKey))
                {
                    return Unauthorized("API_KEY header is required.");
                }

                var message = _crossDockingService.ShipItems(shipmentId, apiKey);
                return Ok(new { message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}