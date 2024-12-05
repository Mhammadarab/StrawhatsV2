using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cargohub.interfaces;
using Cargohub.models;
using Cargohub.services;
using Microsoft.AspNetCore.Mvc;

namespace Cargohub.controllers
{
    [ApiExplorerSettings(GroupName = "Inventories")]
    [Route("api/v1/inventories/")]  
    [ApiController]
    public class InventoryController : Controller
    {
        private readonly ICrudService<Inventory, int> _inventoryService;
        private readonly ICrudService<Item, string> _itemService;
        private readonly ICrudService<Location, int> _locationsService;
        private readonly ICrudService<Warehouse, int> _warehouseService;
        private readonly ICrudService<Classifications, int> _classificationsService;



        public InventoryController(ICrudService<Inventory, int> inventoryService, ICrudService<Item, string> itemService, ICrudService<Location, int> locationsService, ICrudService<Warehouse, int> warehouseService, ICrudService<Classifications, int> classificationsService)
        {
            _inventoryService = inventoryService;
            _itemService = itemService;
            _locationsService = locationsService;
            _warehouseService = warehouseService;
            _classificationsService = classificationsService;

        }

        [HttpGet]
        public IActionResult GetInventories()
        {
            var inventories = _inventoryService.GetAll();
            if (inventories == null || !inventories.Any())
            {
                return NotFound();
            }
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

        [HttpPost]
        public async Task<IActionResult> CreateInventory([FromBody] Inventory inventory)
        {
            if (inventory == null)
            {
                return BadRequest("Inventory data is null.");
            }
            var classifications = _classificationsService.GetAll();
            var hazardousClassification = classifications.First(c => c.Name == "hazardous");
            var item = _itemService.GetById(inventory.Item_Id);
            if (item == null)
            {
                return BadRequest("Item not found");
            }
            // item is hazardous
            if (item.Classifications_Id.Contains(hazardousClassification.Id))
            {
                foreach (var locationId in inventory.Locations.Keys)
                {
                    var isParsed = int.TryParse(locationId, out int parsedLocationId);
                    if (!isParsed) {
                        return BadRequest("Invalid locationId");
                    }
                    var location = _locationsService.GetById(parsedLocationId);
                    if (location == null)
                    {
                        return BadRequest("Location not found");
                    }
                    var warehouse = _warehouseService.GetById(location.Warehouse_Id);
                    if (warehouse == null)
                    {
                        return BadRequest("Warehouse not found");
                    }
                    if (!warehouse.Classifications_Id.Contains(hazardousClassification.Id))
                    {
                        return BadRequest("Warehouse is non-hazardous");
                    }
                }
            }


            await _inventoryService.Create(inventory);
            return CreatedAtAction(nameof(GetInventoryById), new { id = inventory.Id }, inventory);
        }

        [HttpPut("{id}")]
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

        [HttpDelete("{id}")]
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