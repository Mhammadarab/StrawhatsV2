using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cargohub.interfaces;
using Cargohub.models;
using Cargohub.services;
using Microsoft.AspNetCore.Mvc;

namespace Cargohub.Controllers.v1
{
    [Route("api/v1/")]
    [ApiController]
    public class ItemLineController : Controller
    {
        private readonly ICrudService<ItemLine, int> _itemLineService;

        public ItemLineController(ICrudService<ItemLine, int> itemLineService)
        {
            _itemLineService = itemLineService;
        }

        [HttpGet("itemlines")]
        public IActionResult GetItemLines()
        {
            var itemLines = _itemLineService.GetAll();
            if (itemLines == null || !itemLines.Any())
            {
                return NotFound();
            }
            return Ok(itemLines);
        }

        [HttpGet("itemlines/{id}")]
        public IActionResult GetItemLineById(int id)
        {
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

        [HttpPost("itemlines")]
        public async Task<IActionResult> CreateItemLine([FromBody] ItemLine itemLine)
        {
            if (itemLine == null)
            {
                return BadRequest("ItemLine data is null.");
            }

            await _itemLineService.Create(itemLine);
            return CreatedAtAction(nameof(GetItemLineById), new { id = itemLine.Id }, itemLine);
        }

        [HttpPut("itemlines/{id}")]
        public async Task<IActionResult> UpdateItemLine(int id, [FromBody] ItemLine itemLine)
        {
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

        [HttpDelete("itemlines/{id}")]
        public async Task<IActionResult> DeleteItemLine(int id)
        {
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