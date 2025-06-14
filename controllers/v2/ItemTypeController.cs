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
        private IActionResult ValidateApiKeyAndUser(string permission)
      {
        var apiKey = Request.Headers["API_KEY"].FirstOrDefault();
        if (string.IsNullOrEmpty(apiKey))
        {
          return Unauthorized("API_KEY header is missing.");
        }

        var user = AuthProvider.GetUser(apiKey);
        if (user == null || !AuthProvider.HasAccess(user, "item_types", permission))
        {
          return Forbid("You do not have permission to access this resource.");
        }

        return null;
      }

        [HttpGet]
        public IActionResult GetItemTypes([FromQuery] int? pageNumber = null, [FromQuery] int? pageSize = null)
        {
            var validationResult = ValidateApiKeyAndUser("all");
            if (validationResult != null)
            {
            return validationResult;
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
            var validationResult = ValidateApiKeyAndUser("single");
            if (validationResult != null)
            {
                return validationResult;
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
            var validationResult = ValidateApiKeyAndUser("all");
            if (validationResult != null)
            {
                return validationResult;
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
            var validationResult = ValidateApiKeyAndUser("post");
            if (validationResult != null)
            {
                return validationResult;
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
            var validationResult = ValidateApiKeyAndUser("put");
            if (validationResult != null)
            {
                return validationResult;
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
            var validationResult = ValidateApiKeyAndUser("delete");
            if (validationResult != null)
            {
                return validationResult;
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