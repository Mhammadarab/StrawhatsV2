using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cargohub.interfaces;
using Cargohub.models;
using Cargohub.services;
using Microsoft.AspNetCore.Mvc;

namespace StrawhatsV2.controllers.v2
{
    [ApiExplorerSettings(GroupName = "Locations")]
    [Route("api/v2/locations/")]
    [ApiController]
    public class LocationsController : Controller
    {
        private readonly ICrudService<Location, int> _locationService;

        public LocationsController(ICrudService<Location, int> locationService)
        {
            _locationService = locationService;
        }

        private IActionResult ValidateApiKeyAndUser(string permission)
        {
            var apiKey = Request.Headers["API_KEY"].FirstOrDefault();
            if (string.IsNullOrEmpty(apiKey))
            {
                return Unauthorized("API_KEY header is missing.");
            }

            var user = AuthProvider.GetUser(apiKey);
            if (user == null || !AuthProvider.HasAccess(user, "locations", permission))
            {
                return Forbid("You do not have permission to access this resource.");
            }

            return null;
        }

        [HttpGet]
        public IActionResult GetLocations()
        {
            var validationResult = ValidateApiKeyAndUser("all");
            if (validationResult != null)
            {
                return validationResult;
            }

            var locations = _locationService.GetAll();
            if (locations == null || !locations.Any())
            {
                return NotFound();
            }
            return Ok(locations);
        }

        [HttpGet("{id}")]
        public IActionResult GetLocationById(int id)
        {
            var validationResult = ValidateApiKeyAndUser("single");
            if (validationResult != null)
            {
                return validationResult;
            }

            try
            {
                var location = _locationService.GetById(id);
                return Ok(location);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateLocation([FromBody] Location location)
        {
            var validationResult = ValidateApiKeyAndUser("post");
            if (validationResult != null)
            {
                return validationResult;
            }

            if (location == null)
            {
                return BadRequest("Location data is null.");
            }

            await _locationService.Create(location);
            return CreatedAtAction(nameof(GetLocationById), new { id = location.Id }, location);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLocation(int id, [FromBody] Location location)
        {
            var validationResult = ValidateApiKeyAndUser("put");
            if (validationResult != null)
            {
                return validationResult;
            }

            if (location == null || location.Id != id)
            {
                return BadRequest("Location data is null.");
            }

            try
            {
                await _locationService.Update(location);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLocation(int id)
        {
            var validationResult = ValidateApiKeyAndUser("delete");
            if (validationResult != null)
            {
                return validationResult;
            }

            try
            {
                await _locationService.Delete(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}