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
            return Ok(warehouses);
        }

        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok("you are in the warehouse controller");
        }
    }
}