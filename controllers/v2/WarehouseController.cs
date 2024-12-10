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
        [HttpGet("{warehouse_id}/locations")]
        public IActionResult GetWarehouseLocations(int warehouse_id)
        {

            var apiKey = Request.Headers["API_KEY"].FirstOrDefault();
            if (string.IsNullOrEmpty(apiKey))
            {
                return Unauthorized("API_KEY header is missing.");
            }

            var user = AuthProvider.GetUser(apiKey);
            if (user == null || !AuthProvider.HasAccess(user, "warehouses", "get"))
            {
                return Forbid("You do not have permission to delete clients.");
            }

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

            var apiKey = Request.Headers["API_KEY"].FirstOrDefault();
            if (string.IsNullOrEmpty(apiKey))
            {
                return Unauthorized("API_KEY header is missing.");
            }

            var user = AuthProvider.GetUser(apiKey);
            if (user == null || !AuthProvider.HasAccess(user, "warehouses", "get"))
            {
                return Forbid("You do not have permission to delete clients.");
            }

            var warehouses = _warehouseService.GetAll();
            if (warehouses == null || !warehouses.Any())
            {
                return NotFound();
            }
            return Ok(warehouses);
        }

        [HttpGet("{id}")]
        public IActionResult GetWarehouseById(int id)
        {

            var apiKey = Request.Headers["API_KEY"].FirstOrDefault();
            if (string.IsNullOrEmpty(apiKey))
            {
                return Unauthorized("API_KEY header is missing.");
            }

            var user = AuthProvider.GetUser(apiKey);
            if (user == null || !AuthProvider.HasAccess(user, "warehouses", "get"))
            {
                return Forbid("You do not have permission to delete clients.");
            }

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

            var apiKey = Request.Headers["API_KEY"].FirstOrDefault();
            if (string.IsNullOrEmpty(apiKey))
            {
                return Unauthorized("API_KEY header is missing.");
            }

            var user = AuthProvider.GetUser(apiKey);
            if (user == null || !AuthProvider.HasAccess(user, "warehouses", "post"))
            {
                return Forbid("You do not have permission to delete clients.");
            }

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

            var apiKey = Request.Headers["API_KEY"].FirstOrDefault();
            if (string.IsNullOrEmpty(apiKey))
            {
                return Unauthorized("API_KEY header is missing.");
            }

            var user = AuthProvider.GetUser(apiKey);
            if (user == null || !AuthProvider.HasAccess(user, "warehouses", "put"))
            {
                return Forbid("You do not have permission to delete clients.");
            }

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

            var apiKey = Request.Headers["API_KEY"].FirstOrDefault();
            if (string.IsNullOrEmpty(apiKey))
            {
                return Unauthorized("API_KEY header is missing.");
            }

            var user = AuthProvider.GetUser(apiKey);
            if (user == null || !AuthProvider.HasAccess(user, "warehouses", "delete"))
            {
                return Forbid("You do not have permission to delete clients.");
            }

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

            var apiKey = Request.Headers["API_KEY"].FirstOrDefault();
            if (string.IsNullOrEmpty(apiKey))
            {
                return Unauthorized("API_KEY header is missing.");
            }

            var user = AuthProvider.GetUser(apiKey);
            if (user == null || !AuthProvider.HasAccess(user, "warehouses", "get"))
            {
                return Forbid("You do not have permission to delete clients.");
            }

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

            var apiKey = Request.Headers["API_KEY"].FirstOrDefault();
            if (string.IsNullOrEmpty(apiKey))
            {
                return Unauthorized("API_KEY header is missing.");
            }

            var user = AuthProvider.GetUser(apiKey);
            if (user == null || !AuthProvider.HasAccess(user, "warehouses", "get"))
            {
                return Forbid("You do not have permission to delete clients.");
            }
            
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

        [HttpPut("{id}/classifications")]
        public async Task<IActionResult> AddClassificationToWarehouse(int id, [FromBody] List<int> classificationIds)
        {
            var apiKey = Request.Headers["API_KEY"].FirstOrDefault();
            if (string.IsNullOrEmpty(apiKey))
            {
                return Unauthorized("API_KEY header is missing.");
            }

            try
            {
                var warehouse = _warehouseService.GetById(id);
                if (warehouse == null)
                {
                    return NotFound($"Warehouse with ID {id} not found.");
                }

                if (warehouse.Classifications_Id == null)
                {
                    warehouse.Classifications_Id = new List<int>();
                }
                
                await _warehouseService.UpdateClassifications(warehouse);

                return Ok(warehouse);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}