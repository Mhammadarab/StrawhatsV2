using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Cargohub.services;
using Cargohub.models;
using Cargohub.interfaces;

namespace Cargohub.controllers.v2
{
    [ApiExplorerSettings(GroupName = "Inventories")]
    [Route("api/v2/inventories/")]
    [ApiController]
    public class InventoryControllerV2 : Controller
    {
        private readonly InventoryService _inventoryService;
        private readonly ICrudService<Item, string> _itemService;
        private readonly ICrudService<Location, int> _locationsService;
        private readonly ICrudService<Warehouse, int> _warehouseService;
        private readonly ICrudService<Classifications, int> _classificationsService;
        
        public InventoryControllerV2(InventoryService inventoryService, ICrudService<Item, string> itemService, ICrudService<Location, int> locationsService, ICrudService<Warehouse, int> warehouseService, ICrudService<Classifications, int> classificationsService)
        {
            _inventoryService = inventoryService;
            _itemService = itemService;
            _locationsService = locationsService;
            _warehouseService = warehouseService;
            _classificationsService = classificationsService;
        }
        private IActionResult ValidateApiKeyAndUser(string permission)
        {
            var apiKey = Request.Headers["API_KEY"].FirstOrDefault();
            if (string.IsNullOrEmpty(apiKey))
            {
                return Unauthorized("API_KEY header is missing.");
            }

            var user = AuthProvider.GetUser(apiKey);
            if (user == null || !AuthProvider.HasAccess(user, "inventories", permission))
            {
                return Forbid("You do not have permission to access this resource.");
            }

            return null;
        }

        [HttpGet]
        public IActionResult GetInventories([FromQuery] int? pageNumber = null, [FromQuery] int? pageSize = null)
        {

            var validationResult = ValidateApiKeyAndUser("all");
            if (validationResult != null)
            {
                return validationResult;
            }

            // Validate pagination parameters if provided
            if ((pageNumber.HasValue && pageNumber <= 0) || (pageSize.HasValue && pageSize <= 0))
            {
                return BadRequest("Page number and page size must be greater than zero if provided.");
            }

            var inventories = _inventoryService.GetAll(pageNumber, pageSize);

            if (inventories == null || !inventories.Any())
            {
                return NotFound("No inventories found.");
            }

            var totalRecords = _inventoryService.GetAll(null, null).Count; // Total count without pagination

            // Return metadata only if pagination is applied
            if (pageNumber.HasValue && pageSize.HasValue)
            {
                return Ok(new
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalRecords = totalRecords,
                    Inventories = inventories
                });
            }

            // Return plain list if pagination is not applied
            return Ok(inventories);
        }

        [HttpGet("{id}")]
        public IActionResult GetInventoryById(int id)
        {

            var validationResult = ValidateApiKeyAndUser("single");
            if (validationResult != null)
            {
                return validationResult;
            }

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

            var validationResult = ValidateApiKeyAndUser("post");
            if (validationResult != null)
            {
                return validationResult;
            }

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
        public IActionResult UpdateInventory(int id, [FromBody] Inventory inventory)
        {

            var validationResult = ValidateApiKeyAndUser("put");
            if (validationResult != null)
            {
                return validationResult;
            }

            if (inventory == null)
            {
                return BadRequest("Inventory data is null.");
            }

            if (id != inventory.Id)
            {
                return BadRequest("Mismatched inventory ID.");
            }

            try
            {
                var existingInventory = _inventoryService.GetAll().FirstOrDefault(i => i.Id == id);
                if (existingInventory == null)
                {
                    return NotFound($"Inventory with ID {id} not found.");
                }

                // Update inventory
                _inventoryService.Update(inventory);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInventory(int id)
        {
            var validationResult = ValidateApiKeyAndUser("delete");
            if (validationResult != null)
            {
                return validationResult;
            }
            try
            {
                await _inventoryService.Delete(id);
                return NoContent();
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

            // Extract the API_KEY from the headers
            var apiKey = Request.Headers["API_KEY"].FirstOrDefault();
            if (string.IsNullOrEmpty(apiKey))
                return Unauthorized("API_KEY header is required.");

            // Perform the audit operation
            var discrepancies = _inventoryService.AuditInventory(apiKey, physicalCountsByLocation);

            return Ok(new
            {
                Message = "Audit completed. Discrepancies have been logged.",
                Discrepancies = discrepancies
            });
        }
    }
}