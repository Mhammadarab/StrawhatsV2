using Microsoft.AspNetCore.Mvc;

namespace Cargohub.Controllers.v2
{
  [ApiController]
  [Route("api/v2/cross-docking")]
  public class CrossDockingController : ControllerBase
  {
    private readonly CrossDockingService _crossDockingService;

    public CrossDockingController(CrossDockingService crossDockingService)
    {
      _crossDockingService = crossDockingService;
    }

    [HttpPost("receive")]
    public IActionResult ReceiveShipment([FromBody] int shipmentId)
    {
      try
      {
        var result = _crossDockingService.ReceiveShipment(shipmentId);
        return Ok(result);
      }
      catch (KeyNotFoundException ex)
      {
        return NotFound(ex.Message);
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"An error occurred: {ex.Message}");
      }
    }

    [HttpGet("match")]
    public IActionResult MatchItems([FromQuery] int? shipmentId = null, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 100)
    {
      try
      {
        var matches = _crossDockingService.MatchItems(shipmentId, pageNumber, pageSize);
        return Ok(matches);
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"An error occurred: {ex.Message}");
      }
    }


    [HttpPost("ship")]
    public IActionResult ShipItems([FromBody] int shipmentId)
    {
      try
      {
        var result = _crossDockingService.ShipItems(shipmentId);
        return Ok(result);
      }
      catch (KeyNotFoundException ex)
      {
        return NotFound(ex.Message);
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"An error occurred: {ex.Message}");
      }
    }
  }
}
