using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cargohub.interfaces;
using Cargohub.models;
using Cargohub.services;

namespace Cargohub.Controllers.v2
{
    [ApiExplorerSettings(GroupName = "ItemLines")]
    [Route("api/v2/item_lines/")]
    [ApiController]
    public class ItemLineController : Controller
    {
        private readonly ICrudService<ItemLine, int> _itemLineService;
        private readonly ItemLineService _itemLineServiceWithItems;

        public ItemLineController(ICrudService<ItemLine, int> itemLineService, ItemLineService itemLineServiceWithItems)
        {
            _itemLineService = itemLineService;
            _itemLineServiceWithItems = itemLineServiceWithItems;
        }

        private IActionResult ValidateApiKeyAndUser(string permission)
        {
            var apiKey = Request.Headers["API_KEY"].FirstOrDefault();
            if (string.IsNullOrEmpty(apiKey))
            {
                return Unauthorized("API_KEY header is missing.");
            }

            var user = AuthProvider.GetUser(apiKey);
            if (user == null || !AuthProvider.HasAccess(user, "item_lines", permission))
            {
                return Forbid("You do not have permission to access this resource.");
            }

            return null;
        }

        [HttpGet]
        public IActionResult GetItemLines([FromQuery] int? pageNumber = null, [FromQuery] int? pageSize = null)
        {
            var validationResult = ValidateApiKeyAndUser("get");
            if (validationResult != null)
            {
                return validationResult;
            }

            // Validate pagination parameters if provided
            if ((pageNumber.HasValue && pageNumber <= 0) || (pageSize.HasValue && pageSize <= 0))
            {
                return BadRequest("Page number and page size must be greater than zero if provided.");
            }
            var itemLines = _itemLineService.GetAll(pageNumber, pageSize);
            if (itemLines == null || !itemLines.Any())
            {
                return NotFound("No inventories found.");
            }

            var totalRecords = _itemLineService.GetAll(null, null).Count; // Total count without pagination

            // Return metadata only if pagination is applied
            if (pageNumber.HasValue && pageSize.HasValue)
            {
                return Ok(new
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalRecords = totalRecords,
                    ItemLines = itemLines
                });
            }

            // Return plain list if pagination is not applied
            return Ok(itemLines);
        }

        [HttpGet("{id}")]
        public IActionResult GetItemLineById(int id)
        {
            var validationResult = ValidateApiKeyAndUser("get");
            if (validationResult != null)
            {
                return validationResult;
            }

            try
            {
                var itemLine = _itemLineService.GetById(id);
                return Ok(itemLine);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("{id}/items")]
        public IActionResult GetItemsByItemLineId(int id)
        {
            var validationResult = ValidateApiKeyAndUser("get");
            if (validationResult != null)
            {
                return validationResult;
            }

            var items = _itemLineServiceWithItems.GetItemsByItemLineId(id);
            if (items == null || items.Count == 0)
            {
                return NotFound();
            }
            return Ok(items);
        }

        [HttpPost]
        public async Task<IActionResult> CreateItemLine([FromBody] ItemLine itemLine)
        {
            var validationResult = ValidateApiKeyAndUser("post");
            if (validationResult != null)
            {
                return validationResult;
            }

            if (itemLine == null)
            {
                return BadRequest("ItemLine data is null.");
            }

            await _itemLineService.Create(itemLine);
            return CreatedAtAction(nameof(GetItemLineById), new { id = itemLine.Id }, itemLine);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateItemLine(int id, [FromBody] ItemLine itemLine)
        {
            var validationResult = ValidateApiKeyAndUser("put");
            if (validationResult != null)
            {
                return validationResult;
            }

            if (itemLine == null || itemLine.Id != id)
            {
                return BadRequest("Invalid ItemLine data.");
            }

            try
            {
                await _itemLineService.Update(itemLine);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItemLine(int id)
        {
            var validationResult = ValidateApiKeyAndUser("delete");
            if (validationResult != null)
            {
                return validationResult;
            }
            
            try
            {
                await _itemLineService.Delete(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}