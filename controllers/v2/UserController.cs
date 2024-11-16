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
            AuthProvider.AddUser(user);
            return CreatedAtAction(nameof(GetUserByApiKey), new { apiKey = user.ApiKey }, user);
        }

        [HttpPut("{apiKey}")]
        public IActionResult UpdateUser(string apiKey, [FromBody] User updatedUser)
        {
            var user = AuthProvider.GetUser(apiKey);
            if (user == null)
            {
                return NotFound();
            }
            AuthProvider.UpdateUser(apiKey, updatedUser);
            return NoContent();
        }

        [HttpDelete("{apiKey}")]
        public IActionResult DeleteUser(string apiKey)
        {
            var user = AuthProvider.GetUser(apiKey);
            if (user == null)
            {
                return NotFound();
            }
            AuthProvider.DeleteUser(apiKey);
            return NoContent();
        }
    }
}