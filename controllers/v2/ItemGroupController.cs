using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cargohub.services;
using Microsoft.AspNetCore.Mvc;
using Cargohub.interfaces;
using Cargohub.models;

namespace Cargohub.Controllers.v2
  {
    [ApiExplorerSettings(GroupName = "ItemGroups")]
    [Route("api/v2/item_groups/")]
    [ApiController]
    public class ItemGroupController : Controller
    {
      private readonly ICrudService<ItemGroup, int> _itemGroupService;

      public ItemGroupController(ICrudService<ItemGroup, int> itemGroupService)
      {
        _itemGroupService = itemGroupService;
      }

      private IActionResult ValidateApiKeyAndUser(string permission)
      {
        var apiKey = Request.Headers["API_KEY"].FirstOrDefault();
        if (string.IsNullOrEmpty(apiKey))
        {
          return Unauthorized("API_KEY header is missing.");
        }

        var user = AuthProvider.GetUser(apiKey);
        if (user == null || !AuthProvider.HasAccess(user, "item_groups", permission))
        {
          return Forbid("You do not have permission to access this resource.");
        }

        return null;
      }

      [HttpGet]
      public IActionResult GetItemGroups([FromQuery] int? pageNumber = null, [FromQuery] int? pageSize = null)
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
        var itemGroups = _itemGroupService.GetAll(pageNumber, pageSize);
        if (itemGroups == null || !itemGroups.Any())
        {
          return NotFound("No inventories found.");
        }
        var totalRecords = _itemGroupService.GetAll(null, null).Count; // Total count without pagination

        // Return metadata only if pagination is applied
        if (pageNumber.HasValue && pageSize.HasValue)
        {
          return Ok(new
          {
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalRecords = totalRecords,
            ItemGroups = itemGroups
          });
        }

        // Return plain list if pagination is not applied
        return Ok(itemGroups);
      }

      [HttpGet("{id}")]
      public IActionResult GetItemGroupById(int id)
      {
        var validationResult = ValidateApiKeyAndUser("single");
        if (validationResult != null)
        {
          return validationResult;
        }

        try
        {
          var itemGroup = _itemGroupService.GetById(id);
          return Ok(itemGroup);
        }
        catch (KeyNotFoundException ex)
        {
          return NotFound(ex.Message);
        }
      }

      [HttpPost]
      public async Task<IActionResult> CreateItemGroup([FromBody] ItemGroup itemGroup)
      {
        var validationResult = ValidateApiKeyAndUser("post");
        if (validationResult != null)
        {
          return validationResult;
        }

        if (itemGroup == null)
        {
          return BadRequest("ItemGroup data is null.");
        }

        await _itemGroupService.Create(itemGroup);
        return CreatedAtAction(nameof(GetItemGroupById), new { id = itemGroup.Id }, itemGroup);
      }

      [HttpPut("{id}")]
      public async Task<IActionResult> UpdateItemGroup(int id, [FromBody] ItemGroup itemGroup)
      {
        var validationResult = ValidateApiKeyAndUser("put");
        if (validationResult != null)
        {
          return validationResult;
        }

        if (itemGroup == null || itemGroup.Id != id)
        {
          return BadRequest("Invalid ItemGroup data.");
        }

        try
        {
          await _itemGroupService.Update(itemGroup);
          return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
          return NotFound(ex.Message);
        }
      }

      [HttpDelete("{id}")]
      public async Task<IActionResult> DeleteItemGroup(int id)
      {
        var validationResult = ValidateApiKeyAndUser("delete");
        if (validationResult != null)
        {
          return validationResult;
        }

        try
        {
          await _itemGroupService.Delete(id);
          return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
          return NotFound(ex.Message);
        }
      }
    }
  }