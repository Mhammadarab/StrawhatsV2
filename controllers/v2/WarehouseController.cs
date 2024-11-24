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
        private readonly ICrudService<Warehouse, int> _warehouseService;
        public WarehouseController(ICrudService<Warehouse, int> warehouseService)
        {
            _warehouseService = warehouseService;
        }

        [HttpGet("capacities")]
        public IActionResult GetAllWarehouseCapacities(int pageNumber = 1, int pageSize = 10)
        {
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
    }
}