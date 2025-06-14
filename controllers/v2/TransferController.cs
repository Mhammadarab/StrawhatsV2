using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cargohub.interfaces;
using Cargohub.models;
using Cargohub.services;
using Microsoft.AspNetCore.Mvc;

namespace Cargohub.controllers.v2
{
  [ApiExplorerSettings(GroupName = "Transfers")]
  [Route("api/v2/transfers/")]
  [ApiController]
  public class TransferController : Controller
  {
    private readonly ICrudService<Transfer, int> _transferService;

    public TransferController(ICrudService<Transfer, int> transferService)
    {
      _transferService = transferService;
    }

    private IActionResult ValidateApiKeyAndUser(string permission)
    {
        var apiKey = Request.Headers["API_KEY"].FirstOrDefault();
        if (string.IsNullOrEmpty(apiKey))
        {
            return Unauthorized("API_KEY header is missing.");
        }

        var user = AuthProvider.GetUser(apiKey);
        if (user == null || !AuthProvider.HasAccess(user, "transfers", permission))
        {
            return Forbid("You do not have permission to access this resource.");
        }

        return null;
    }

    [HttpGet]
    public IActionResult GetTransfers([FromQuery] int? pageNumber = null, [FromQuery] int? pageSize = null)
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

        var transfers = _transferService.GetAll(pageNumber, pageSize);

        if (transfers == null || !transfers.Any())
        {
            return NotFound("No transfers found.");
        }

        // Include pagination metadata if pagination is applied
        if (pageNumber.HasValue && pageSize.HasValue)
        {
            var totalRecords = _transferService.GetAll(null, null).Count;
            return Ok(new
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalRecords,
                Transfers = transfers
            });
        }

        // Return plain list if pagination is not applied
        return Ok(transfers);
    }


    [HttpGet("{id}")]
    public IActionResult GetTransferById(int id)
    {
        var validationResult = ValidateApiKeyAndUser("single");
        if (validationResult != null)
        {
            return validationResult;
        }

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
        var validationResult = ValidateApiKeyAndUser("all");
        if (validationResult != null)
        {
            return validationResult;
        }

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
        var validationResult = ValidateApiKeyAndUser("post");
        if (validationResult != null)
        {
            return validationResult;
        }

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
        var validationResult = ValidateApiKeyAndUser("put");
        if (validationResult != null)
        {
            return validationResult;
        }

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
        var validationResult = ValidateApiKeyAndUser("delete");
        if (validationResult != null)
        {
            return validationResult;
        }

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
