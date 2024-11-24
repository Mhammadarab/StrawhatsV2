using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cargohub.interfaces;
using Cargohub.models;

namespace Cargohub.Controllers.v2
{
    [ApiExplorerSettings(GroupName = "ItemLines")]
    [Route("api/v2/item_lines")]
    [ApiController]
    public class ItemLineController : Controller
    {
        [HttpGet]
        public IActionResult GetItemLines()
        {
            return Ok("List of all Item Lines in v2");
        }
    }
}