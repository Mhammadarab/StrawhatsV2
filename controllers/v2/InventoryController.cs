using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cargohub.services;
using Microsoft.AspNetCore.Mvc;

namespace Cargohub.controllers.v2
{
  [Route("api/v2/inventories")]
  [ApiController]
  public class InventoryController : Controller
  {
    private readonly InventoryService _inventoryService;

    public InventoryController(InventoryService inventoryService)
    {
      _inventoryService = inventoryService;
    }

    [HttpPost("audit")]
    public IActionResult AuditInventoryByLocation([FromBody] Dictionary<int, Dictionary<int, int>> physicalCountsByLocation)
    {
      if (physicalCountsByLocation == null || !physicalCountsByLocation.Any())
      {
        return BadRequest("Physical counts by location data is required.");
      }

      var discrepancies = _inventoryService.AuditInventoryByLocation(physicalCountsByLocation);

      if (!discrepancies.Any())
      {
        return Ok("No discrepancies found.");
      }

      return Ok(discrepancies);
    }
  }
}