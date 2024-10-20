using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cargohub.interfaces;
using Cargohub.models;
using Microsoft.AspNetCore.Mvc;

namespace Cargohub.controllers
{
  [Route("api/v1/")]
  [ApiController]
  public class ItemGroupController : Controller
  {
    private readonly ICrudService<ItemGroup, int> _itemGroupService;

    public ItemGroupController(ICrudService<ItemGroup, int> itemGroupService)
    {
      _itemGroupService = itemGroupService;
    }

    [HttpGet("item_groups")]
    public IActionResult GetItemGroups()
    {
      var itemGroups = _itemGroupService.GetAll();
      if (itemGroups == null || !itemGroups.Any())
      {
        return NotFound();
      }
      return Ok(itemGroups);
    }

    [HttpGet("item_groups/{id}")]
    public IActionResult GetItemGroupById(int id)
    {
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

    [HttpPost("item_groups")]
    public async Task<IActionResult> CreateItemGroup([FromBody] ItemGroup itemGroup)
    {
      if (itemGroup == null)
      {
        return BadRequest("ItemGroup data is null.");
      }

      await _itemGroupService.Create(itemGroup);
      return CreatedAtAction(nameof(GetItemGroupById), new { id = itemGroup.Id }, itemGroup);
    }

    [HttpPut("item_groups/{id}")]
    public async Task<IActionResult> UpdateItemGroup(int id, [FromBody] ItemGroup itemGroup)
    {
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

    [HttpDelete("item_groups/{id}")]
    public async Task<IActionResult> DeleteItemGroup(int id)
    {
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
