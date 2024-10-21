using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cargohub.interfaces;
using Cargohub.models;

namespace Cargohub.Controllers.v1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ItemController : Controller
    {
        private readonly ICrudService<Item, string> _itemService;

        public ItemController(ICrudService<Item, string> itemService)
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

        [HttpGet("itemline/{itemLineId}")]
        public IActionResult GetItemsForItemLine(int itemLineId)
        {
            var items = _itemService.GetItemsForItemLine(itemLineId);
            return Ok(items);
        }

        [HttpGet("itemgroup/{itemGroupId}")]
        public IActionResult GetItemsForItemGroup(int itemGroupId)
        {
            var items = _itemService.GetItemsForItemGroup(itemGroupId);
            return Ok(items);
        }

        [HttpGet("itemtype/{itemTypeId}")]
        public IActionResult GetItemsForItemType(int itemTypeId)
        {
            var items = _itemService.GetItemsForItemType(itemTypeId);
            return Ok(items);
        }

        [HttpGet("supplier/{supplierId}")]
        public IActionResult GetItemsForSupplier(int supplierId)
        {
            var items = _itemService.GetItemsForSupplier(supplierId);
            return Ok(items);
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