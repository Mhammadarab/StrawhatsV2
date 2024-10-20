using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cargohub.interfaces;
using Cargohub.models;

namespace Cargohub.Controllers.v2
{
    [Route("api/v2/[controller]")]
    [ApiController]
    public class ItemLineController : Controller
    {
        private readonly ICrudService<ItemLine, int> _itemLineService;

        public ItemLineController(ICrudService<ItemLine, int> itemLineService)
        {
            _itemLineService = itemLineService;
        }

        [HttpGet]
        public IActionResult GetItemLines()
        {
            var itemLines = _itemLineService.GetAll();
            return Ok(itemLines);
        }

        [HttpGet("{id}")]
        public IActionResult GetItemLineById(int id)
        {
            var itemLine = _itemLineService.GetById(id);
            if (itemLine == null)
            {
                return NotFound();
            }
            return Ok(itemLine);
        }

        [HttpPost]
        public async Task<IActionResult> CreateItemLine([FromBody] ItemLine itemLine)
        {
            if (itemLine == null)
            {
                return BadRequest();
            }

            await _itemLineService.Create(itemLine);
            return CreatedAtAction(nameof(GetItemLineById), new { id = itemLine.Id }, itemLine);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateItemLine(int id, [FromBody] ItemLine itemLine)
        {
            if (itemLine == null || id != itemLine.Id)
            {
                return BadRequest();
            }

            try
            {
                await _itemLineService.Update(itemLine);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItemLine(int id)
        {
            try
            {
                await _itemLineService.Delete(id);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}