using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cargohub.interfaces;
using Cargohub.models;

namespace Cargohub.Controllers.v1
{
    [ApiExplorerSettings(GroupName = "Items")]
    [Route("api/v1/items/")]
    [ApiController]
    public class ItemController : Controller
    {
        private readonly IItemService _itemService;

        public ItemController(IItemService itemService)
        {
            _itemService = itemService;
        }

        [HttpGet]
        public IActionResult GetItems()
        {
            var items = _itemService.GetAll();
            return Ok(items);
        }

        [HttpGet("{uid}")]
        public IActionResult GetItemById(string uid)
        {
            var item = _itemService.GetById(uid);
            if (item == null)
            {
                return NotFound();
            }
            return Ok(item);
        }

        [HttpGet("{itemId}/inventory")]
        public IActionResult GetItemInventory(string itemId)
        {
            return Ok(new List<object>());
        }

        [HttpGet("{itemId}/inventory/totals")]
        public IActionResult GetItemInventoryTotals(string itemId)
        {
            try
            {
                var inventoryTotals = _itemService.GetItemInventoryTotals(itemId);
                return Ok(inventoryTotals);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateItem([FromBody] Item item)
        {
            if (item == null)
            {
                return BadRequest();
            }

            await _itemService.Create(item);
            return CreatedAtAction(nameof(GetItemById), new { uid = item.Uid }, item);
        }

        [HttpPut("{uid}")]
        public async Task<IActionResult> UpdateItem(string uid, [FromBody] Item item)
        {
            if (item == null || uid != item.Uid)
            {
                return BadRequest();
            }

            try
            {
                await _itemService.Update(item);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{uid}")]
        public async Task<IActionResult> DeleteItem(string uid)
        {
            try
            {
                await _itemService.Delete(uid);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}