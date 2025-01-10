using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cargohub.interfaces;
using Cargohub.models;
using Cargohub.services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;

namespace Cargohub.controllers.v2
{
    [ApiExplorerSettings(GroupName = "Classifications")]
    [Route("api/v2/classifications/")]
    [ApiController]
    public class ClassificationsController : Controller
    {
        private readonly ICrudService<Classifications, int> _classificationService;
        public ClassificationsController(ICrudService<Classifications, int> classificationService)
        {
            _classificationService = classificationService;
        }
        private IActionResult ValidateApiKeyAndUser(string permission)
        {
            var apiKey = Request.Headers["API_KEY"].FirstOrDefault();
            if (string.IsNullOrEmpty(apiKey))
            {
                return Unauthorized("API_KEY header is missing.");
            }

            var user = AuthProvider.GetUser(apiKey);
            if (user == null || !AuthProvider.HasAccess(user, "classifications", permission))
            {
                return Forbid("You do not have permission to access this resource.");
            }

            return null;
        }

        [HttpGet]
        public IActionResult GetClassifications()
        {
            var validationResult = ValidateApiKeyAndUser("all");
            if (validationResult != null)
            {
                return validationResult;
            }
            var classifications = _classificationService.GetAll();
            if (classifications == null || !classifications.Any())
            {
                return NotFound();
            }
            return Ok(classifications);
        }

        [HttpGet("{id}")]
        public IActionResult GetClassificationById(int id)
        {
            var validationResult = ValidateApiKeyAndUser("single");
            if (validationResult != null)
            {
                return validationResult;
            }

            try
            {
                var classification = _classificationService.GetById(id);
                return Ok(classification);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }


        [HttpPost]
        public async Task<IActionResult> CreateClassification([FromBody] Classifications classification)
        {

            var validationResult = ValidateApiKeyAndUser("post");
            if (validationResult != null)
            {
                return validationResult;
            }


            if (classification == null)
            {
                return BadRequest("Classification data is null.");
            }

            await _classificationService.Create(classification);
            return CreatedAtAction(nameof(GetClassificationById), new { id = classification.Id }, classification);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateClassification(int id, [FromBody] Classifications classification)
        {

            var validationResult = ValidateApiKeyAndUser("put");
            if (validationResult != null)
            {
                return validationResult;
            }

            if (classification == null || classification.Id != id)
            {
                return BadRequest("Invalid classification data.");
            }

            try
            {
                await _classificationService.Update(classification);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClassification(int id)
        {

            var validationResult = ValidateApiKeyAndUser("delete");
            if (validationResult != null)
            {
                return validationResult;
            }

            try
            {
                await _classificationService.Delete(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}