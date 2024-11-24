using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
                return NotFound();
            }
            return Ok(user);
        }

        [HttpPost]
        public IActionResult CreateUser([FromBody] User user)
        {
            try
            {
                AuthProvider.AddUser(user);
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
            try
            {
                AuthProvider.UpdateUser(apiKey, updatedUser);
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

        [HttpDelete("{apiKey}")]
        public IActionResult DeleteUser(string apiKey)
        {
            try
            {
                AuthProvider.DeleteUser(apiKey);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        private async Task LogChange(string action, string targetApiKey, User oldUser, User newUser)
        {
            var logMessage = $"API_KEY {User.Identity.Name} {action} API_KEY {targetApiKey}";

            if (oldUser != null && newUser != null)
            {
                logMessage += $"\nOld User: {JsonConvert.SerializeObject(oldUser, Formatting.Indented)}";
                logMessage += $"\nNew User: {JsonConvert.SerializeObject(newUser, Formatting.Indented)}";
            }
            else if (oldUser != null)
            {
                logMessage += $"\nDeleted User: {JsonConvert.SerializeObject(oldUser, Formatting.Indented)}";
            }
            else if (newUser != null)
            {
                logMessage += $"\nCreated User: {JsonConvert.SerializeObject(newUser, Formatting.Indented)}";
            }

            await System.IO.File.AppendAllTextAsync("log.txt", logMessage + "\n");
        }
    }
}