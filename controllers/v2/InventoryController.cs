using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Cargohub.services;
using Cargohub.models;

namespace Cargohub.controllers.v2
{
    [Route("api/v2/inventories/")]
    [ApiController]
    public class InventoryControllerV2 : Controller
    {
        private readonly InventoryService _inventoryService;

        public InventoryControllerV2(InventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        [HttpGet]
        public IActionResult GetInventories()
        {
            var inventories = _inventoryService.GetAll();
            if (inventories == null || inventories.Count == 0)
                return NotFound("No inventories found.");

            return Ok(inventories);
        }

        [HttpGet("{id}")]
        public IActionResult GetInventoryById(int id)
        {
            try
            {
                var inventory = _inventoryService.GetById(id);
                return Ok(inventory);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("audit")]
        public IActionResult AuditInventory([FromBody] Dictionary<int, Dictionary<int, int>> physicalCountsByLocation)
        {
            if (physicalCountsByLocation == null || physicalCountsByLocation.Count == 0)
                return BadRequest("Audit data is empty.");

            var discrepancies = _inventoryService.AuditInventory(physicalCountsByLocation);
            return Ok(new
            {
                Message = "Audit completed.",
                Discrepancies = discrepancies
            });
        }
    }
}
