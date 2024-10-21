using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cargohub.interfaces;
using Cargohub.models;
using Microsoft.AspNetCore.Mvc;

namespace Cargohub.controllers
{
    [Route("api/v1/")]
    [ApiController]
    public class InventoryController : Controller
    {
        private readonly ICrudService<Inventory, int> _inventoryService;
    
        public InventoryController(ICrudService<Inventory, int> inventoryService)
        {
            _inventoryService = inventoryService;
        }
    
        [HttpGet("inventories")]
        public IActionResult GetInventories()
        {
        var inventories = _inventoryService.GetAll();
        if (inventories == null || !inventories.Any())
        {
            return NotFound();
        }
        return Ok(inventories);
        }
    
        [HttpGet("inventories/{id}")]
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
    
        [HttpPost("inventories")]
        public async Task<IActionResult> CreateInventory([FromBody] Inventory inventory)
        {
        if (inventory == null)
        {
            return BadRequest("Inventory data is null.");
        }
    
        await _inventoryService.Create(inventory);
        return CreatedAtAction(nameof(GetInventoryById), new { id = inventory.Id }, inventory);
        }
    
        [HttpPut("inventories/{id}")]
        public async Task<IActionResult> UpdateInventory([FromBody] Inventory inventory)
        {
        if (inventory == null)
        {
            return BadRequest("Inventory data is null.");
        }
    
        try
        {
            await _inventoryService.Update(inventory);
            return Ok(inventory);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        }
    
        [HttpDelete("inventories/{id}")]
        public async Task<IActionResult> DeleteInventory(int id)
        {
        try
        {
            await _inventoryService.Delete(id);
            return Ok("Inventory deleted successfully.");
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        }
    }
}