using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cargohub.interfaces;
using Cargohub.models;
using Cargohub.services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;

namespace Cargohub.controllers.v2
{
    [ApiExplorerSettings(GroupName = "Clients")]
    [Route("api/v2/clients/")]
    [ApiController]
    public class ClientsController : Controller
    {
        private readonly ICrudService<Client, int> _clientService;
        private readonly ICrudService<Order, int> _orderService;

        public ClientsController(ICrudService<Client, int> clientService, ICrudService<Order, int> orderService)
        {
            _clientService = clientService;
            _orderService = orderService;
        }


        [HttpGet]
        public IActionResult GetClients([FromQuery] int? pageNumber = null, [FromQuery] int? pageSize = null)
        {
            // Validate pagination parameters if provided
            if ((pageNumber.HasValue && pageNumber <= 0) || (pageSize.HasValue && pageSize <= 0))
            {
                return BadRequest("Page number and page size must be greater than zero if provided.");
            }

            var clients = _clientService.GetAll(pageNumber, pageSize);

            if (clients == null || !clients.Any())
            {
                return NotFound();
            }

            var totalRecords = _clientService.GetAll(null, null).Count; // Total count without pagination

            // Return metadata only if pagination is applied
            if (pageNumber.HasValue && pageSize.HasValue)
            {
                return Ok(new
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalRecords = totalRecords,
                    Clients = clients
                });
            }
            return Ok(clients);
        }
        
        [HttpGet("{id}/orders")]
        public async Task<IActionResult> GetClientOrder(int id)
        {
            try
            {
                var client = _clientService.GetById(id);
                var orders = _orderService.GetAll().FindAll( o => o.Ship_To == client.Id ||o.Bill_To == client.Id);
                return Ok(orders);
            }
            catch(KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetClientById(int id)
        {
            try
            {
                var client = _clientService.GetById(id);
                return Ok(client);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateClient([FromBody] Client client)
        {
            if (client == null)
            {
                return BadRequest("Client data is null.");
            }

            await _clientService.Create(client);
            return CreatedAtAction(nameof(GetClientById), new { id = client.Id }, client);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateClient([FromBody] Client client)
        {
            if (client == null)
            {
                return BadRequest("Client data is null.");
            }

            try
            {
                await _clientService.Update(client);
                return Ok(client);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClient(int id)
        {
            try
            {
                await _clientService.Delete(id);
                return Ok();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}