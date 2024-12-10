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
    [ApiExplorerSettings(GroupName = "Shipments")]
    [Route("api/v2/shipments/")]
    [ApiController]
    public class ShipmentController : Controller
    {
        private readonly ICrudService<Shipment, int> _shipmentService;

        private readonly ICrudService<Order, int> _orderService;

        public ShipmentController(ICrudService<Shipment, int> shipmentService, ICrudService<Order, int> orderService)
        {
            _shipmentService = shipmentService;
            _orderService = orderService;
        }

        [HttpGet]
        public IActionResult GetShipments([FromQuery] int? pageNumber = null, [FromQuery] int? pageSize = null)
        {
            var apiKey = Request.Headers["API_KEY"].FirstOrDefault();
            if (string.IsNullOrEmpty(apiKey))
            {
                return Unauthorized("API_KEY header is missing.");
            }

            var user = AuthProvider.GetUser(apiKey);
            if (user == null || !AuthProvider.HasAccess(user, "shipments", "get"))
            {
                return Forbid("You do not have permission to delete clients.");
            }
            
            // Validate pagination parameters if provided
            if ((pageNumber.HasValue && pageNumber <= 0) || (pageSize.HasValue && pageSize <= 0))
            {
                return BadRequest("Page number and page size must be greater than zero if provided.");
            }

            var shipments = _shipmentService.GetAll(pageNumber, pageSize);

            if (shipments == null || !shipments.Any())
            {
                return NotFound("No shipments found.");
            }

            // Include pagination metadata if pagination is applied
            if (pageNumber.HasValue && pageSize.HasValue)
            {
                var totalRecords = _shipmentService.GetAll(null, null).Count;
                return Ok(new
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalRecords = totalRecords,
                    Shipments = shipments
                });
            }

            // Return plain list if pagination is not applied
            return Ok(shipments);
}


        [HttpGet("{id}")]
        public IActionResult GetShipmentById(int id)
        {
            var apiKey = Request.Headers["API_KEY"].FirstOrDefault();
            if (string.IsNullOrEmpty(apiKey))
            {
                return Unauthorized("API_KEY header is missing.");
            }

            var user = AuthProvider.GetUser(apiKey);
            if (user == null || !AuthProvider.HasAccess(user, "shipments", "get"))
            {
                return Forbid("You do not have permission to delete clients.");
            }

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

        [HttpPost]
        public async Task<IActionResult> CreateShipment([FromBody] Shipment shipment)
        {
            var apiKey = Request.Headers["API_KEY"].FirstOrDefault();
            if (string.IsNullOrEmpty(apiKey))
            {
                return Unauthorized("API_KEY header is missing.");
            }

            var user = AuthProvider.GetUser(apiKey);
            if (user == null || !AuthProvider.HasAccess(user, "shipments", "post"))
            {
                return Forbid("You do not have permission to delete clients.");
            }

            if (shipment == null)
            {
                return BadRequest("Shipment data is null.");
            }

            await _shipmentService.Create(shipment);
            return CreatedAtAction(nameof(GetShipmentById), new { id = shipment.Id }, shipment);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateShipment(int id, [FromBody] Shipment shipment)
        {
            var apiKey = Request.Headers["API_KEY"].FirstOrDefault();
            if (string.IsNullOrEmpty(apiKey))
            {
                return Unauthorized("API_KEY header is missing.");
            }

            var user = AuthProvider.GetUser(apiKey);
            if (user == null || !AuthProvider.HasAccess(user, "shipments", "put"))
            {
                return Forbid("You do not have permission to delete clients.");
            }

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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShipment(int id)
        {
            var apiKey = Request.Headers["API_KEY"].FirstOrDefault();
            if (string.IsNullOrEmpty(apiKey))
            {
                return Unauthorized("API_KEY header is missing.");
            }

            var user = AuthProvider.GetUser(apiKey);
            if (user == null || !AuthProvider.HasAccess(user, "shipments", "delete"))
            {
                return Forbid("You do not have permission to delete clients.");
            }

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

        [HttpGet("{id}/items")]
        public async Task<IActionResult> GetShipmentItems(int id)
        {
            var apiKey = Request.Headers["API_KEY"].FirstOrDefault();
            if (string.IsNullOrEmpty(apiKey))
            {
                return Unauthorized("API_KEY header is missing.");
            }

            var user = AuthProvider.GetUser(apiKey);
            if (user == null || !AuthProvider.HasAccess(user, "shipments", "get"))
            {
                return Forbid("You do not have permission to delete clients.");
            }

            try
            {
                var shipment = _shipmentService.GetById(id);
                return Ok(shipment.Items);
            }
            catch (KeyNotFoundException e)
            {

                return NotFound(e.Message);
            }
        }

        [HttpPut("{id}/items")]
        public async Task<IActionResult> UpdateShipmentItems(int id, [FromBody] Shipment shipmentBody)
        {
            var apiKey = Request.Headers["API_KEY"].FirstOrDefault();
            if (string.IsNullOrEmpty(apiKey))
            {
                return Unauthorized("API_KEY header is missing.");
            }

            var user = AuthProvider.GetUser(apiKey);
            if (user == null || !AuthProvider.HasAccess(user, "shipments", "put"))
            {
                return Forbid("You do not have permission to delete clients.");
            }

            try
            {
                var shipment = _shipmentService.GetById(id);
                shipment.Items = shipmentBody.Items;
                await _shipmentService.Update(shipment);
                return NoContent();
            }
            catch (KeyNotFoundException e)
            {

                return NotFound(e.Message);
            }
        }

        [HttpGet("{id}/orders")]
        public async Task<IActionResult> GetShipmentOrder(int id)
        {
            var apiKey = Request.Headers["API_KEY"].FirstOrDefault();
            if (string.IsNullOrEmpty(apiKey))
            {
                return Unauthorized("API_KEY header is missing.");
            }

            var user = AuthProvider.GetUser(apiKey);
            if (user == null || !AuthProvider.HasAccess(user, "shipments", "get"))
            {
                return Forbid("You do not have permission to delete clients.");
            }

            try
            {
                var shipment = _shipmentService.GetById(id);
                var order = _orderService.GetById(shipment.Order_Id);
                return Ok(order);
            }
            catch (KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }
        }

        [HttpPut("{id}/orders")]
        public async Task<IActionResult> UpdateShipmentOrder(int id, [FromBody] Order order)
        {
            var apiKey = Request.Headers["API_KEY"].FirstOrDefault();
            if (string.IsNullOrEmpty(apiKey))
            {
                return Unauthorized("API_KEY header is missing.");
            }

            var user = AuthProvider.GetUser(apiKey);
            if (user == null || !AuthProvider.HasAccess(user, "shipments", "put"))
            {
                return Forbid("You do not have permission to delete clients.");
            }
            
            try
            {
                if (order == null) return BadRequest("order is missing");

                var targetOrder = _orderService.GetById(order.Id);
                var targetShipment = _shipmentService.GetById(id);
                targetShipment.Order_Id = targetOrder.Id;
                targetShipment.Order_Date = targetOrder.Order_Date;
                await _shipmentService.Update(targetShipment);
                return NoContent();
            }
            catch (KeyNotFoundException e)
            {

                return NotFound(e);
            }

        }
    }
}