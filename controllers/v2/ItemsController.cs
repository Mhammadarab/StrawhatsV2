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

        [HttpGet]
        public IActionResult GetItems()
        {
            var apiKey = Request.Headers["API_KEY"].FirstOrDefault();
            if (string.IsNullOrEmpty(apiKey))
            {
                return Unauthorized("API_KEY header is missing.");
            }

            var user = AuthProvider.GetUser(apiKey);
            if (user == null || !AuthProvider.HasAccess(user, "items", "get"))
            {
                return Forbid("You do not have permission to delete clients.");
            }
            
            var items = _itemService.GetAll();
            return Ok(items);
        }

        [HttpGet("{uid}")]
        public IActionResult GetItemById(string uid)
        {
            var apiKey = Request.Headers["API_KEY"].FirstOrDefault();
            if (string.IsNullOrEmpty(apiKey))
            {
                return Unauthorized("API_KEY header is missing.");
            }

            var user = AuthProvider.GetUser(apiKey);
            if (user == null || !AuthProvider.HasAccess(user, "items", "get"))
            {
                return Forbid("You do not have permission to delete clients.");
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
            var apiKey = Request.Headers["API_KEY"].FirstOrDefault();
            if (string.IsNullOrEmpty(apiKey))
            {
                return Unauthorized("API_KEY header is missing.");
            }

            var user = AuthProvider.GetUser(apiKey);
            if (user == null || !AuthProvider.HasAccess(user, "items", "get"))
            {
                return Forbid("You do not have permission to delete clients.");
            }

            return Ok(new List<object>());
        }

        [HttpGet("{itemId}/inventory/totals")]
        public IActionResult GetItemInventoryTotals(string itemId)
        {
            var apiKey = Request.Headers["API_KEY"].FirstOrDefault();
            if (string.IsNullOrEmpty(apiKey))
            {
                return Unauthorized("API_KEY header is missing.");
            }

            var user = AuthProvider.GetUser(apiKey);
            if (user == null || !AuthProvider.HasAccess(user, "items", "get"))
            {
                return Forbid("You do not have permission to delete clients.");
            }

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
            var apiKey = Request.Headers["API_KEY"].FirstOrDefault();
            if (string.IsNullOrEmpty(apiKey))
            {
                return Unauthorized("API_KEY header is missing.");
            }

            var user = AuthProvider.GetUser(apiKey);
            if (user == null || !AuthProvider.HasAccess(user, "items", "post"))
            {
                return Forbid("You do not have permission to delete clients.");
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
            var apiKey = Request.Headers["API_KEY"].FirstOrDefault();
            if (string.IsNullOrEmpty(apiKey))
            {
                return Unauthorized("API_KEY header is missing.");
            }

            var user = AuthProvider.GetUser(apiKey);
            if (user == null || !AuthProvider.HasAccess(user, "items", "put"))
            {
                return Forbid("You do not have permission to delete clients.");
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
            var apiKey = Request.Headers["API_KEY"].FirstOrDefault();
            if (string.IsNullOrEmpty(apiKey))
            {
                return Unauthorized("API_KEY header is missing.");
            }

            var user = AuthProvider.GetUser(apiKey);
            if (user == null || !AuthProvider.HasAccess(user, "items", "delete"))
            {
                return Forbid("You do not have permission to delete clients.");
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
            var apiKey = Request.Headers["API_KEY"].FirstOrDefault();
            if (string.IsNullOrEmpty(apiKey))
            {
                return Unauthorized("API_KEY header is missing.");
            }

            var user = AuthProvider.GetUser(apiKey);
            if (user == null || !AuthProvider.HasAccess(user, "items", "put"))
            {
                return Forbid("You do not have permission to update item classifications.");
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