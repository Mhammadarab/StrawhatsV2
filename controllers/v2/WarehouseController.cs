using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Cargohub.controllers.v2
{
    [Route("api/v2/[controller]")]
    [ApiController]
    public class WarehouseController : Controller
    {
        [HttpGet]
        public IActionResult GetWarehouses()
        {
            return Ok("List of all warehouses in v2");
        }
    }
}