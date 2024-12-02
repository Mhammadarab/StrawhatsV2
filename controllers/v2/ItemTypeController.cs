using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cargohub.interfaces;
using Cargohub.models;
using Cargohub.services;

namespace Cargohub.Controllers.v2
{
    [ApiExplorerSettings(GroupName = "ItemTypes")]
    [Route("api/v2/item_types/")]
    [ApiController]
    public class ItemTypeController : Controller
    {
        private readonly ICrudService<ItemType, int> _itemTypeService;

        public ItemTypeController(ICrudService<ItemType, int> itemTypeService)
        {
            _itemTypeService = itemTypeService;
        }

        [HttpGet]
        public IActionResult GetItemTypes()
        {
            var itemTypes = _itemTypeService.GetAll();
            if (itemTypes == null || !itemTypes.Any())
            {
                return NotFound();
            }
            return Ok(itemTypes);
        }

        [HttpGet("{id}")]
        public IActionResult GetItemTypeById(int id)
        {
            try
            {
                var itemType = _itemTypeService.GetById(id);
                return Ok(itemType);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("{id}/items")]
        public IActionResult GetItemsByItemTypeId(int id)
        {
            var items = ((ItemTypeService)_itemTypeService).GetItemsByItemTypeId(id);
            if (items == null || items.Count == 0)
            {
                return NotFound();
            }
            return Ok(items);
        }

        [HttpPost]
        public async Task<IActionResult> CreateItemType([FromBody] ItemType itemType)
        {
            if (itemType == null)
            {
                return BadRequest("ItemType data is null.");
            }

            await _itemTypeService.Create(itemType);
            return CreatedAtAction(nameof(GetItemTypeById), new { id = itemType.Id }, itemType);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateItemType(int id, [FromBody] ItemType itemType)
        {
            if (itemType == null || itemType.Id != id)
            {
                return BadRequest("Invalid ItemType data.");
            }

            try
            {
                await _itemTypeService.Update(itemType);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItemType(int id)
        {
            try
            {
                await _itemTypeService.Delete(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}