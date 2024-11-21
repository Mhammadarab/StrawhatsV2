using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cargohub.models;
using Cargohub.services;

namespace Cargohub.Controllers.v2
{
    [Route("api/v2/[controller]")]
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
    }
}