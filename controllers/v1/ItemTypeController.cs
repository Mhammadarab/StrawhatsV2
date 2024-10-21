using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cargohub.interfaces;
using Cargohub.models;

namespace Cargohub.Controllers.v1
{
    [Route("api/v1/[controller]")]
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
            return Ok(itemTypes);
        }

        [HttpGet("{id}")]
        public IActionResult GetItemTypeById(int id)
        {
            var itemType = _itemTypeService.GetById(id);
            if (itemType == null)
            {
                return NotFound();
            }
            return Ok(itemType);
        }

        [HttpPost]
        public async Task<IActionResult> CreateItemType([FromBody] ItemType itemType)
        {
            if (itemType == null)
            {
                return BadRequest();
            }

            await _itemTypeService.Create(itemType);
            return CreatedAtAction(nameof(GetItemTypeById), new { id = itemType.Id }, itemType);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateItemType(int id, [FromBody] ItemType itemType)
        {
            if (itemType == null || id != itemType.Id)
            {
                return BadRequest();
            }

            try
            {
                await _itemTypeService.Update(itemType);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItemType(int id)
        {
            try
            {
                await _itemTypeService.Delete(id);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}