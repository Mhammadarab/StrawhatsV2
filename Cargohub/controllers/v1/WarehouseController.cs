using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Cargohub.controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class WarehouseController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetWarehouses()
        {
            return Ok("List of all warehouses");
        }
    }
}