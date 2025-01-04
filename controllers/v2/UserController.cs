using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Cargohub.models;
using Cargohub.services;

namespace Cargohub.Controllers.v2
{
    [ApiExplorerSettings(GroupName = "Users")]
    [Route("api/v2/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetUsers()
        {
            var users = AuthProvider.GetUsers();
            return Ok(users);
        }

        [HttpGet("{apiKey}")]
        public IActionResult GetUserByApiKey(string apiKey)
        {
            var user = AuthProvider.GetUser(apiKey);
            if (user == null)
            {
                return NotFound("User not found.");
            }
            return Ok(user);
        }

        [HttpPost]
        public IActionResult CreateUser([FromBody] User user)
        {
            var adminApiKey = Request.Headers["API_KEY"].FirstOrDefault();
            if (string.IsNullOrEmpty(adminApiKey))
            {
                return Unauthorized("API_KEY header is required for authorization.");
            }

            if (user == null || string.IsNullOrEmpty(user.ApiKey))
            {
                return BadRequest("User data or API key is missing.");
            }

            try
            {
                AuthProvider.AddUser(adminApiKey, user);
                return CreatedAtAction(nameof(GetUserByApiKey), new { apiKey = user.ApiKey }, user);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{apiKey}")]
        public IActionResult UpdateUser(string apiKey, [FromBody] User updatedUser)
        {
            var adminApiKey = Request.Headers["API_KEY"].FirstOrDefault();
            if (string.IsNullOrEmpty(adminApiKey))
            {
                return Unauthorized("API_KEY header is required for authorization.");
            }

            if (updatedUser == null)
            {
                return BadRequest("User data is null.");
            }

            try
            {
                AuthProvider.UpdateUser(adminApiKey, apiKey, updatedUser);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{apiKey}")]
        public IActionResult DeleteUser(string apiKey)
        {
            var adminApiKey = Request.Headers["API_KEY"].FirstOrDefault();
            if (string.IsNullOrEmpty(adminApiKey))
            {
                return Unauthorized("API_KEY header is required for authorization.");
            }

            try
            {
                AuthProvider.DeleteUser(adminApiKey, apiKey);
                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpPost("{apiKey}/deactivate")]
        public IActionResult DeactivateUser(string apiKey)
        {
            var adminApiKey = Request.Headers["API_KEY"].FirstOrDefault();
            if (string.IsNullOrEmpty(adminApiKey))
            {
                return Unauthorized("API_KEY header is required for authorization.");
            }

            try
            {
                AuthProvider.DeactivateUser(adminApiKey, apiKey);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("{apiKey}/reactivate")]
        public IActionResult ReactivateUser(string apiKey)
        {
            var adminApiKey = Request.Headers["API_KEY"].FirstOrDefault();
            if (string.IsNullOrEmpty(adminApiKey))
            {
                return Unauthorized("API_KEY header is required for authorization.");
            }

            try
            {
                AuthProvider.ReactivateUser(adminApiKey, apiKey);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("{apiKey}/warehouses")]
        public IActionResult GetWarehouses(string apiKey)
        {
            var user = AuthProvider.GetUser(apiKey);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            return Ok(user.Warehouses);
        }

        [HttpPut("{apiKey}/warehouses/add")]
        public IActionResult AddWarehouse(string apiKey, [FromBody] int warehouseId)
        {
            var adminApiKey = Request.Headers["API_KEY"].FirstOrDefault();
            if (string.IsNullOrEmpty(adminApiKey))
            {
                return Unauthorized("API_KEY header is required for authorization.");
            }

            try
            {
                AuthProvider.AddWarehouse(adminApiKey, apiKey, warehouseId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{apiKey}/warehouses/remove")]
        public IActionResult RemoveWarehouse(string apiKey, [FromBody] int warehouseId)
        {
            var adminApiKey = Request.Headers["API_KEY"].FirstOrDefault();
            if (string.IsNullOrEmpty(adminApiKey))
            {
                return Unauthorized("API_KEY header is required for authorization.");
            }

            try
            {
                AuthProvider.RemoveWarehouse(adminApiKey, apiKey, warehouseId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
