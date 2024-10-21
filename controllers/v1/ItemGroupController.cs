using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cargohub.interfaces;
using Cargohub.models;
using Microsoft.AspNetCore.Mvc;

namespace Cargohub.controllers
{
  [Route("api/v1/item_groups/")]
  [ApiController]
  public class ItemGroupController : Controller
  {
    private readonly ICrudService<ItemGroup, int> _itemGroupService;

    public ItemGroupController(ICrudService<ItemGroup, int> itemGroupService)
    {
      _itemGroupService = itemGroupService;
    }

    [HttpGet]
    public IActionResult GetItemGroups()
    {
      var itemGroups = _itemGroupService.GetAll();
      if (itemGroups == null || !itemGroups.Any())
      {
        return NotFound();
      }
      return Ok(itemGroups);
    }

    [HttpGet("{id}")]
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

    [HttpPost]
    public async Task<IActionResult> CreateItemGroup([FromBody] ItemGroup itemGroup)
    {
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
