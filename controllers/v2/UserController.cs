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

        private IActionResult ValidateApiKeyAndUser(string permission)
        {
            var apiKey = Request.Headers["API_KEY"].FirstOrDefault();
            if (string.IsNullOrEmpty(apiKey))
            {
                return Unauthorized("API_KEY header is missing.");
            }

            var user = AuthProvider.GetUser(apiKey);
            if (user == null || !AuthProvider.HasAccess(user, "users", permission))
            {
                return Forbid("You do not have permission to access this resource.");
            }

            return null;
        }

        [HttpPost]
        public IActionResult CreateUser([FromBody] User user)
        {
            var validationResult = ValidateApiKeyAndUser("post");
            if (validationResult != null)
            {
                return validationResult;
            }

            if (user == null || string.IsNullOrEmpty(user.ApiKey))
            {
                return BadRequest("User data or API key is missing.");
            }

            try
            {
                AuthProvider.AddUser(Request.Headers["API_KEY"].FirstOrDefault(), user);
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
            var validationResult = ValidateApiKeyAndUser("put");
            if (validationResult != null)
            {
                return validationResult;
            }

            if (updatedUser == null)
            {
                return BadRequest("User data is null.");
            }

            try
            {
                AuthProvider.UpdateUser(Request.Headers["API_KEY"].FirstOrDefault(), apiKey, updatedUser);
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
            var validationResult = ValidateApiKeyAndUser("delete");
            if (validationResult != null)
            {
                return validationResult;
            }

            try
            {
                AuthProvider.DeleteUser(Request.Headers["API_KEY"].FirstOrDefault(), apiKey);
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
        // TODO: Check this 
        [HttpPost("{apiKey}/deactivate")]
        public IActionResult DeactivateUser(string apiKey)
        {
            var validationResult = ValidateApiKeyAndUser("deactivate");
            if (validationResult != null)
            {
                return validationResult;
            }

            try
            {
                AuthProvider.DeactivateUser(Request.Headers["API_KEY"].FirstOrDefault(), apiKey);
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
            var validationResult = ValidateApiKeyAndUser("reactivate");
            if (validationResult != null)
            {
                return validationResult;
            }

            try
            {
                AuthProvider.ReactivateUser(Request.Headers["API_KEY"].FirstOrDefault(), apiKey);
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
            var validationResult = ValidateApiKeyAndUser("add_warehouse");
            if (validationResult != null)
            {
                return validationResult;
            }

            try
            {
                AuthProvider.AddWarehouse(Request.Headers["API_KEY"].FirstOrDefault(), apiKey, warehouseId);
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
            var validationResult = ValidateApiKeyAndUser("remove_warehouse");
            if (validationResult != null)
            {
                return validationResult;
            }

            try
            {
                AuthProvider.RemoveWarehouse(Request.Headers["API_KEY"].FirstOrDefault(), apiKey, warehouseId);
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
