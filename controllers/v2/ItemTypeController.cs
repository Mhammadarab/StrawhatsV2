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
        public IActionResult GetItemTypes([FromQuery] int? pageNumber = null, [FromQuery] int? pageSize = null)
        {
            var apiKey = Request.Headers["API_KEY"].FirstOrDefault();
            if (string.IsNullOrEmpty(apiKey))
            {
                return Unauthorized("API_KEY header is missing.");
            }

            var user = AuthProvider.GetUser(apiKey);
            if (user == null || !AuthProvider.HasAccess(user, "item_types", "get"))
            {
                return Forbid("You do not have permission to delete clients.");
            }

            // Validate pagination parameters if provided
            if ((pageNumber.HasValue && pageNumber <= 0) || (pageSize.HasValue && pageSize <= 0))
            {
                return BadRequest("Page number and page size must be greater than zero if provided.");
            }
            var itemTypes = _itemTypeService.GetAll(pageNumber, pageSize);
            if (itemTypes == null || !itemTypes.Any())
            {
                return NotFound("No inventories found.");
            }

            var totalRecords = _itemTypeService.GetAll(null, null).Count; // Total count without pagination

            // Return metadata only if pagination is applied
            if (pageNumber.HasValue && pageSize.HasValue)
            {
                return Ok(new
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalRecords = totalRecords,
                    ItemTypes = itemTypes
                });
            }

            // Return plain list if pagination is not applied
            return Ok(itemTypes);
        }

        [HttpGet("{id}")]
        public IActionResult GetItemTypeById(int id)
        {
            var apiKey = Request.Headers["API_KEY"].FirstOrDefault();
            if (string.IsNullOrEmpty(apiKey))
            {
                return Unauthorized("API_KEY header is missing.");
            }

            var user = AuthProvider.GetUser(apiKey);
            if (user == null || !AuthProvider.HasAccess(user, "item_types", "get"))
            {
                return Forbid("You do not have permission to delete clients.");
            }

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
            var apiKey = Request.Headers["API_KEY"].FirstOrDefault();
            if (string.IsNullOrEmpty(apiKey))
            {
                return Unauthorized("API_KEY header is missing.");
            }

            var user = AuthProvider.GetUser(apiKey);
            if (user == null || !AuthProvider.HasAccess(user, "item_types", "get"))
            {
                return Forbid("You do not have permission to delete clients.");
            }

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
            var apiKey = Request.Headers["API_KEY"].FirstOrDefault();
            if (string.IsNullOrEmpty(apiKey))
            {
                return Unauthorized("API_KEY header is missing.");
            }

            var user = AuthProvider.GetUser(apiKey);
            if (user == null || !AuthProvider.HasAccess(user, "item_types", "post"))
            {
                return Forbid("You do not have permission to delete clients.");
            }

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
            var apiKey = Request.Headers["API_KEY"].FirstOrDefault();
            if (string.IsNullOrEmpty(apiKey))
            {
                return Unauthorized("API_KEY header is missing.");
            }

            var user = AuthProvider.GetUser(apiKey);
            if (user == null || !AuthProvider.HasAccess(user, "item_types", "put"))
            {
                return Forbid("You do not have permission to delete clients.");
            }

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
            var apiKey = Request.Headers["API_KEY"].FirstOrDefault();
            if (string.IsNullOrEmpty(apiKey))
            {
                return Unauthorized("API_KEY header is missing.");
            }

            var user = AuthProvider.GetUser(apiKey);
            if (user == null || !AuthProvider.HasAccess(user, "item_types", "delete"))
            {
                return Forbid("You do not have permission to delete clients.");
            }
            
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