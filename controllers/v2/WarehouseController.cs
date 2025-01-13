using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cargohub.interfaces;
using Cargohub.models;
using Cargohub.services;
using Microsoft.AspNetCore.Mvc;

namespace Cargohub.controllers.v2
{
    [ApiExplorerSettings(GroupName = "Warehouses")]
    [Route("api/v2/warehouses")]
    [ApiController]
    public class WarehouseController : Controller
    {
        private readonly WarehouseService _warehouseService;
        public WarehouseController(WarehouseService warehouseService)
        {
            _warehouseService = warehouseService;
        }

        private IActionResult ValidateApiKeyAndUser(string permission)
        {
            var apiKey = Request.Headers["API_KEY"].FirstOrDefault();
            if (string.IsNullOrEmpty(apiKey))
            {
                return Unauthorized("API_KEY header is missing.");
            }

            var user = AuthProvider.GetUser(apiKey);
            if (user == null || !AuthProvider.HasAccess(user, "warehouses", permission))
            {
                return Forbid("You do not have permission to access this resource.");
            }

            return null;
        }
        
        [HttpGet("{warehouse_id}/locations")]
        public IActionResult GetWarehouseLocations(int warehouse_id)
        {
            var validationResult = ValidateApiKeyAndUser("all");
            if (validationResult != null) return validationResult;

            try
            {
                var locations = ((WarehouseService)_warehouseService).GetWarehouseLocations(warehouse_id);
                if (locations == null || !locations.Any())
                {
                    return NotFound($"No locations found for Warehouse ID {warehouse_id}");
                }
                return Ok(locations);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
        
        [HttpGet]
        public IActionResult GetWarehouses()
        {
            var validationResult = ValidateApiKeyAndUser("all");
            if (validationResult != null) return validationResult;

            var warehouses = _warehouseService.GetAll();
            if (warehouses == null)
            {
                return NotFound();
            }
            return Ok(warehouses);
        }

        [HttpGet("{id}")]
        public IActionResult GetWarehouseById(int id)
        {
            var validationResult = ValidateApiKeyAndUser("single");
            if (validationResult != null) return validationResult;
            

            try
            {
                var warehouse = _warehouseService.GetById(id);
                return Ok(warehouse);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateWarehouse([FromBody] Warehouse warehouse)
        {
            var validationResult = ValidateApiKeyAndUser("post");
            if (validationResult != null) return validationResult;

            if (warehouse == null)
            {
                return BadRequest("Warehouse data is null.");
            }

            await _warehouseService.Create(warehouse);
            return CreatedAtAction(nameof(GetWarehouseById), new { id = warehouse.Id }, warehouse);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateWarehouse(int id, [FromBody] Warehouse warehouse)
        {
            var validationResult = ValidateApiKeyAndUser("put");
            if (validationResult != null) return validationResult;

            if (warehouse == null || warehouse.Id != id)
            {
                return BadRequest("Invalid warehouse data.");
            }

            try
            {
                await _warehouseService.Update(warehouse);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWarehouse(int id)
        {
            var validationResult = ValidateApiKeyAndUser("delete");
            if (validationResult != null) return validationResult;

            try
            {
                await _warehouseService.Delete(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("capacities")]
        public IActionResult GetAllWarehouseCapacities(int pageNumber = 1, int pageSize = 10)
        {
            var validationResult = ValidateApiKeyAndUser("all");
            if (validationResult != null) return validationResult;

            try
            {
                var capacities = ((WarehouseService)_warehouseService).CalculateAllWarehouseCapacities(pageNumber, pageSize);
                return Ok(new
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = ((WarehouseService)_warehouseService).GetAll().Count,
                    Data = capacities
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpGet("{id}/capacities")]
        public IActionResult GetWarehouseCapacities(int id)
        {
            var validationResult = ValidateApiKeyAndUser("all");
            if (validationResult != null) return validationResult;
            
            try
            {
                var (totalCapacity, currentCapacity) = ((WarehouseService)_warehouseService).CalculateWarehouseCapacities(id);
                return Ok(new
                {
                    WarehouseId = id,
                    TotalCapacity = totalCapacity,
                    CurrentCapacity = currentCapacity
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpPut("{id}/add-classifications")]
        public IActionResult AddClassificationsToWarehouse(int id, [FromBody] List<int> classificationIds)
        {
            var validationResult = ValidateApiKeyAndUser("put");
            if (validationResult != null) return validationResult;

            try
            {
                var updatedWarehouse = _warehouseService.AddClassifications(id, classificationIds);
                return Ok(updatedWarehouse); // Return the updated warehouse
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}