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
    [Route("api/v1/")]
    [ApiController]
    public class ClientsController : Controller
    {
        private readonly ICrudService<Client, int> _clientService;

        public ClientsController(ICrudService<Client, int> clientService)
        {
            _clientService = clientService;
        }

        [HttpGet("clients")]
        public IActionResult GetClients()
        {
            var clients = _clientService.GetAll();
            if (clients == null || !clients.Any())
            {
                return NotFound();
            }
            return Ok(clients);
        }

        [HttpGet("clients/{id}")]
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

        [HttpPost("clients")]
        public async Task<IActionResult> CreateClient([FromBody] Client client)
        {
            if (client == null)
            {
                return BadRequest("Client data is null.");
            }

            await _clientService.Create(client);
            return CreatedAtAction(nameof(GetClientById), new { id = client.Id }, client);
        }

        [HttpPut("clients/{id}")]
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

        [HttpDelete("clients/{id}")]
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