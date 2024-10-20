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
    [Route("api/v1/")]
    [ApiController]
    public class WarehouseController : Controller
    {
        private readonly ICrudService<Warehouse, int> _warehouseService;

        public WarehouseController(ICrudService<Warehouse, int> warehouseService)
        {
            _warehouseService = warehouseService;
        }

        [HttpGet("warehouses")]
        public IActionResult GetWarehouses()
        {
            var warehouses = _warehouseService.GetAll();
            if (warehouses == null || !warehouses.Any())
            {
                return NotFound();
            }
            return Ok(warehouses);
        }

        [HttpGet("warehouses/{id}")]
        public IActionResult GetWarehouseById(int id)
        {
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

        [HttpPost("warehouses")]
        public async Task<IActionResult> CreateWarehouse([FromBody] Warehouse warehouse)
        {
            if (warehouse == null)
            {
                return BadRequest("Warehouse data is null.");
            }

            await _warehouseService.Create(warehouse);
            return CreatedAtAction(nameof(GetWarehouseById), new { id = warehouse.Id }, warehouse);
        }

        [HttpPut("warehouses/{id}")]
        public async Task<IActionResult> UpdateWarehouse(int id, [FromBody] Warehouse warehouse)
        {
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

        [HttpDelete("warehouses/{id}")]
        public async Task<IActionResult> DeleteWarehouse(int id)
        {
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
    }
}
