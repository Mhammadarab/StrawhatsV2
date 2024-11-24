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
    [Route("api/v2/warehouses")]
    [ApiController]
    public class WarehouseController : Controller
    {
        private readonly ICrudService<Warehouse, int> _warehouseService;
        public WarehouseController(ICrudService<Warehouse, int> warehouseService)
        {
            _warehouseService = warehouseService;
        }
        [HttpGet]
        public IActionResult GetWarehouses()
        {
            return Ok("List of all warehouses in v2");
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