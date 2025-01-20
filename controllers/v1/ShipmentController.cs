using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cargohub.interfaces;
using Cargohub.models;
using Microsoft.AspNetCore.Mvc;

namespace Cargohub.controllers
{
    [ApiExplorerSettings(GroupName = "Shipments")]
    [Route("api/v1/shipments/")]
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
        public IActionResult GetShipments()
        {
            var shipments = _shipmentService.GetAll();
            if (shipments == null || !shipments.Any())
            {
                return NotFound();
            }
            return Ok(shipments);
        }

        [HttpGet("{id}")]
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

        [HttpPost]
        public async Task<IActionResult> CreateShipment([FromBody] Shipment shipment)
        {
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
            try
            {
                var shipment = _shipmentService.GetById(id);
                var orders = new List<Order>();
                foreach (var orderId in shipment.Order_Id)
                {
                    var order = _orderService.GetById(orderId);
                    orders.Add(order);
                }
                return Ok(orders);
            }
            catch (KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }
        }

        [HttpPut("{id}/orders")]
        public async Task<IActionResult> UpdateShipmentOrder(int id, [FromBody] Order order)
        {
            try
            {
                if (order == null) return BadRequest("order is missing");

                var targetOrder = _orderService.GetById(order.Id);
                var targetShipment = _shipmentService.GetById(id);
                targetShipment.Order_Id = new List<int> { targetOrder.Id };
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