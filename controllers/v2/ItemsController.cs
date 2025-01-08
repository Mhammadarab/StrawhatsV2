using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cargohub.interfaces;
using Cargohub.models;
using Cargohub.services;

namespace Cargohub.Controllers.v2
{
    [ApiExplorerSettings(GroupName = "Items")]
    [Route("api/v2/items/")]
    [ApiController]
    public class ItemController : Controller
    {
        private readonly ItemService _itemService;

        public ItemController(ItemService itemService)
        {
            _itemService = itemService;
        }

        private IActionResult ValidateApiKeyAndUser(string permission)
        {
            var apiKey = Request.Headers["API_KEY"].FirstOrDefault();
            if (string.IsNullOrEmpty(apiKey))
            {
                return Unauthorized("API_KEY header is missing.");
            }

            var user = AuthProvider.GetUser(apiKey);
            if (user == null || !AuthProvider.HasAccess(user, "items", permission))
            {
                return Forbid("You do not have permission to access this resource.");
            }

            return null;
        }

        [HttpGet]
        public IActionResult GetItems()
        {
            var validationResult = ValidateApiKeyAndUser("all");
            if (validationResult != null)
            {
                return validationResult;
            }

            var items = _itemService.GetAll();
            return Ok(items);
        }

        [HttpGet("{uid}")]
        public IActionResult GetItemById(string uid)
        {
            var validationResult = ValidateApiKeyAndUser("single");
            if (validationResult != null)
            {
                return validationResult;
            }

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
            var validationResult = ValidateApiKeyAndUser("single");
            if (validationResult != null)
            {
                return validationResult;
            }

            return Ok(new List<object>());
        }

        [HttpGet("{itemId}/inventory/totals")]
        public IActionResult GetItemInventoryTotals(string itemId)
        {
            var validationResult = ValidateApiKeyAndUser("all");
            if (validationResult != null)
            {
                return validationResult;
            }

            try
            {
                var inventoryTotals = _itemService.GetTotalInventory(itemId);
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
            var validationResult = ValidateApiKeyAndUser("post");
            if (validationResult != null)
            {
                return validationResult;
            }
            
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
            var validationResult = ValidateApiKeyAndUser("put");
            if (validationResult != null)
            {
                return validationResult;
            }

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
            var validationResult = ValidateApiKeyAndUser("delete");
            if (validationResult != null)
            {
                return validationResult;
            }
            
            try
            {
                await _itemService.Delete(uid);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }

        }
        [HttpPut("{uid}/add-classifications")]
        public IActionResult AddClassificationsToItem(string uid, [FromBody] List<int> classificationIds)
        {
            var validationResult = ValidateApiKeyAndUser("put");
            if (validationResult != null)
            {
                return validationResult;
            }

            try
            {
                var updatedItem = _itemService.AddClassifications(uid, classificationIds);
                return Ok(updatedItem); // Return the updated item
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}