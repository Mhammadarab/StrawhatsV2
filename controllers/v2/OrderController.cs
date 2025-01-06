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
    [ApiExplorerSettings(GroupName = "Orders")]
    [Route("api/v2/orders/")]
    [ApiController]
    public class OrderContoller : Controller
    {
        private readonly ICrudService<Order, int> _orderService;
        public OrderContoller(ICrudService<Order, int> orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public IActionResult GetOrders([FromQuery] int? pageNumber = null, [FromQuery] int? pageSize = null)
        {
            var apiKey = Request.Headers["API_KEY"].FirstOrDefault();
            if (string.IsNullOrEmpty(apiKey))
            {
                return Unauthorized("API_KEY header is missing.");
            }

            var user = AuthProvider.GetUser(apiKey);
            if (user == null || !AuthProvider.HasAccess(user, "orders", "get"))
            {
                return Forbid("You do not have permission to delete clients.");
            }

            if ((pageNumber.HasValue && pageNumber <= 0) || (pageSize.HasValue && pageSize <= 0))
            {
                return BadRequest("Page number and page size must be greater than zero if provided.");
            }

            var orders = _orderService.GetAll(pageNumber, pageSize);

            if (orders == null || !orders.Any())
            {
                return NotFound();
            }

            var totalRecords = _orderService.GetAll(null, null).Count;

            if (pageNumber.HasValue && pageSize.HasValue)
            {
                return Ok(new
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalRecords = totalRecords,
                    Orders = orders
                });
            }
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public IActionResult GetOrderById(int id)
        {
            var apiKey = Request.Headers["API_KEY"].FirstOrDefault();
            if (string.IsNullOrEmpty(apiKey))
            {
                return Unauthorized("API_KEY header is missing.");
            }

            var user = AuthProvider.GetUser(apiKey);
            if (user == null || !AuthProvider.HasAccess(user, "orders", "get"))
            {
                return Forbid("You do not have permission to delete clients.");
            }

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
            var apiKey = Request.Headers["API_KEY"].FirstOrDefault();
            if (string.IsNullOrEmpty(apiKey))
            {
                return Unauthorized("API_KEY header is missing.");
            }

            var user = AuthProvider.GetUser(apiKey);
            if (user == null || !AuthProvider.HasAccess(user, "orders", "post"))
            {
                return Forbid("You do not have permission to create orders.");
            }

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
            var apiKey = Request.Headers["API_KEY"].FirstOrDefault();
            if (string.IsNullOrEmpty(apiKey))
            {
                return Unauthorized("API_KEY header is missing.");
            }

            var user = AuthProvider.GetUser(apiKey);
            if (user == null || !AuthProvider.HasAccess(user, "orders", "put"))
            {
                return Forbid("You do not have permission to update orders.");
            }

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
            var apiKey = Request.Headers["API_KEY"].FirstOrDefault();
            if (string.IsNullOrEmpty(apiKey))
            {
                return Unauthorized("API_KEY header is missing.");
            }

            var user = AuthProvider.GetUser(apiKey);
            if (user == null || !AuthProvider.HasAccess(user, "orders", "delete"))
            {
                return Forbid("You do not have permission to delete clients.");
            }

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
            var apiKey = Request.Headers["API_KEY"].FirstOrDefault();
            if (string.IsNullOrEmpty(apiKey))
            {
                return Unauthorized("API_KEY header is missing.");
            }

            var user = AuthProvider.GetUser(apiKey);
            if (user == null || !AuthProvider.HasAccess(user, "orders", "get"))
            {
                return Forbid("You do not have permission to delete clients.");
            }

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
            var apiKey = Request.Headers["API_KEY"].FirstOrDefault();
            if (string.IsNullOrEmpty(apiKey))
            {
                return Unauthorized("API_KEY header is missing.");
            }

            var user = AuthProvider.GetUser(apiKey);
            if (user == null || !AuthProvider.HasAccess(user, "orders", "put"))
            {
                return Forbid("You do not have permission to delete clients.");
            }

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