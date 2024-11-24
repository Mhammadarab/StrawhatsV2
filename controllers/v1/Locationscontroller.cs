using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cargohub.interfaces;
using Cargohub.models;
using Cargohub.services;
using Microsoft.AspNetCore.Mvc;

namespace Cargohub.Controllers.v1
{
    [ApiExplorerSettings(GroupName = "Locations")]
    [Route("api/v1/locations/")]
    [ApiController]
    public class LocationsController : Controller
    {
        private readonly ICrudService<Location, int> _locationService;

        public LocationsController(ICrudService<Location, int> locationService)
        {
            _locationService = locationService;
        }

        [HttpGet]
        public IActionResult GetLocations()
        {
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
            if (location == null || location.Id != id)
            {
                return BadRequest("Location data is null.");
            }

            try
            {
                await _locationService.Update(location);
                return Ok(location);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLocation(int id)
        {
            try
            {
                await _locationService.Delete(id);
                return Ok();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}