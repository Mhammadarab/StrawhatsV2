using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cargohub.interfaces;
using Cargohub.models;
using Cargohub.services;
using Microsoft.AspNetCore.Mvc;

namespace Cargohub.controllers
{
  [Route("api/v1/transfers/")]
  [ApiController]
  public class TransferController : Controller
  {
    private readonly ICrudService<Transfer, int> _transferService;

    public TransferController(ICrudService<Transfer, int> transferService)
    {
      _transferService = transferService;
    }

    [HttpGet]
    public IActionResult GetTransfers()
    {
      var transfers = _transferService.GetAll();
      if (transfers == null || !transfers.Any())
      {
        return NotFound();
      }
      return Ok(transfers);
    }

    [HttpGet("{id}")]
    public IActionResult GetTransferById(int id)
    {
      try
      {
        var transfer = _transferService.GetById(id);
        return Ok(transfer);
      }
      catch (KeyNotFoundException ex)
      {
        return NotFound(ex.Message);
      }
    }

    [HttpGet("transfers/{transfer_id}/items")]
    public IActionResult GetTransferItems(int transfer_id)
      {
        try
        {
            var items = ((TransferService)_transferService).GetTransferItems(transfer_id);
            if (items == null || !items.Any())
            {
                return NotFound($"No items found for Transfer ID {transfer_id}");
            }
            return Ok(items);
        }
        catch (KeyNotFoundException ex)

        {
          return NotFound(ex.Message);
        }
      }

    [HttpPost]
    public async Task<IActionResult> CreateTransfer([FromBody] Transfer transfer)
    {
      if (transfer == null)
      {
        return BadRequest("Transfer data is null.");
      }

      await _transferService.Create(transfer);
      return CreatedAtAction(nameof(GetTransferById), new { id = transfer.Id }, transfer);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTransfer(int id, [FromBody] Transfer transfer)
    {
      if (transfer == null || transfer.Id != id)
      {
        return BadRequest("Invalid transfer data.");
      }

      try
      {
        await _transferService.Update(transfer);
        return NoContent();
      }
      catch (KeyNotFoundException ex)
      {
        return NotFound(ex.Message);
      }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTransfer(int id)
    {
      try
      {
        await _transferService.Delete(id);
        return NoContent();
      }
      catch (KeyNotFoundException ex)
      {
        return NotFound(ex.Message);
      }
    }
  }
}
