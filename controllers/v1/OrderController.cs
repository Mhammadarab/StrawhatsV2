using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cargohub.interfaces;
using Cargohub.models;
using Cargohub.services;
using Microsoft.AspNetCore.Mvc;

namespace Cargohub.controllers.v1
{
    [ApiExplorerSettings(GroupName = "Orders")]
    [Route("api/v1/orders/")]
    [ApiController]
    public class OrderContoller : Controller
    {
        private readonly ICrudService<Order, int> _orderService;
        public OrderContoller(ICrudService<Order, int> orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public IActionResult GetOrders()
        {
            var orders = _orderService.GetAll();
            if (orders == null || !orders.Any())
            {
                return NotFound();
            }
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public IActionResult GetOrderById(int id)
        {
            try
            {
                var order = _orderService.GetById(id);
                return Ok(order);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] Order order)
        {
            if (order == null)
            {
                return BadRequest("Order data is null.");
            }

            await _orderService.Create(order);
            return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, order);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] Order order)
        {
            if (order == null || order.Id != id)
            {
                return BadRequest("Order data is invalid.");
            }

            try
            {
                await _orderService.Update(order);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            try
            {
                await _orderService.Delete(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("{id}/items")]
        public async Task<IActionResult> GetOrderItems(int id)
        {
            try
            {
                var order = _orderService.GetById(id);
                return Ok(order.Items);
            }
            catch (KeyNotFoundException ex)
            {

                return NotFound(ex.Message);
            }
        }

        [HttpPut("{id}/items")]
        public async Task<IActionResult> UpdateOrderItems(int id, [FromBody] Order orderBody)
        {
            try
            {
                var targetOrder = _orderService.GetById(id);
                targetOrder.Items = orderBody.Items;
                await _orderService.Update(targetOrder);
                return NoContent();
            }
            catch (KeyNotFoundException e)
            {

                return NotFound(e.Message);
            }
        }
    }
}