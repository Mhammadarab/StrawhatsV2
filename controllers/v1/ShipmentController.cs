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
    public class ShipmentController : Controller
    {
        private readonly ICrudService<Shipment, int> _shipmentService;

        public ShipmentController(ICrudService<Shipment, int> shipmentService)
        {
            _shipmentService = shipmentService;
        }

        [HttpGet("shipments")]
        public IActionResult GetShipments()
        {
            var shipments = _shipmentService.GetAll();
            if (shipments == null || !shipments.Any())
            {
                return NotFound();
            }
            return Ok(shipments);
        }

        [HttpGet("shipments/{id}")]
        public IActionResult GetShipmentById(int id)
        {
            try
            {
                var shipment = _shipmentService.GetById(id);
                return Ok(shipment);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("shipments")]
        public async Task<IActionResult> CreateShipment([FromBody] Shipment shipment)
        {
            if (shipment == null)
            {
                return BadRequest("Shipment data is null.");
            }

            await _shipmentService.Create(shipment);
            return CreatedAtAction(nameof(GetShipmentById), new { id = shipment.Id }, shipment);
        }

        [HttpPut("shipments/{id}")]
        public async Task<IActionResult> UpdateShipment(int id, [FromBody] Shipment shipment)
        {
            if (shipment == null || shipment.Id != id)
            {
                return BadRequest("Shipment data is invalid.");
            }

            try
            {
                await _shipmentService.Update(shipment);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("shipments/{id}")]
        public async Task<IActionResult> DeleteShipment(int id)
        {
            try
            {
                await _shipmentService.Delete(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}