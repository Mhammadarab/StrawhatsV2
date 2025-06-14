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
    [ApiExplorerSettings(GroupName = "Suppliers")]
    [Route("api/v2/suppliers/")]
    [ApiController]
    public class SupplierController : Controller
    {
        private readonly ICrudService<Supplier, int> _supplierService;

        public SupplierController(ICrudService<Supplier, int> supplierService)
        {
            _supplierService = supplierService;
        }

        private IActionResult ValidateApiKeyAndUser(string permission)
        {
            var apiKey = Request.Headers["API_KEY"].FirstOrDefault();
            if (string.IsNullOrEmpty(apiKey))
            {
                return Unauthorized("API_KEY header is missing.");
            }

            var user = AuthProvider.GetUser(apiKey);
            if (user == null || !AuthProvider.HasAccess(user, "suppliers", permission))
            {
                return Forbid("You do not have permission to access this resource.");
            }

            return null;
        }

        [HttpGet]
        public IActionResult GetSuppliers([FromQuery] int? pageNumber = null, [FromQuery] int? pageSize = null)
        {
            var validationResult = ValidateApiKeyAndUser("all");
            if (validationResult != null)
            {
                return validationResult;
            }

            if ((pageNumber.HasValue && pageNumber <= 0) || (pageSize.HasValue && pageSize <= 0))
            {
                return BadRequest("Page number and page size must be greater than zero if provided.");
            }

            var suppliers = _supplierService.GetAll(pageNumber, pageSize);

            if (suppliers == null || !suppliers.Any())
            {
                return NotFound();
            }

            var totalRecords = _supplierService.GetAll(null, null).Count;

            if (pageNumber.HasValue && pageSize.HasValue)
            {
                return Ok(new
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalRecords = totalRecords,
                    Suppliers = suppliers
                });
            }
            return Ok(suppliers);
        }

        [HttpGet("{id}")]
        public IActionResult GetSupplierById(int id)
        {
            var validationResult = ValidateApiKeyAndUser("single");
            if (validationResult != null)
            {
                return validationResult;
            }

            try
            {
                var supplier = _supplierService.GetById(id);
                return Ok(supplier);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateSupplier([FromBody] Supplier supplier)
        {
            var validationResult = ValidateApiKeyAndUser("post");
            if (validationResult != null)
            {
                return validationResult;
            }

            if (supplier == null)
            {
                return BadRequest("Supplier data is null.");
            }

            await _supplierService.Create(supplier);
            return CreatedAtAction(nameof(GetSupplierById), new { id = supplier.Id }, supplier);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSupplier(int id, [FromBody] Supplier supplier)
        {
            var validationResult = ValidateApiKeyAndUser("put");
            if (validationResult != null)
            {
                return validationResult;
            }

            if (supplier == null || supplier.Id != id)
            {
                return BadRequest("Supplier data is invalid.");
            }

            try
            {
                await _supplierService.Update(supplier);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSupplier(int id)
        {
            var validationResult = ValidateApiKeyAndUser("delete");
            if (validationResult != null)
            {
                return validationResult;
            }

            try
            {
                await _supplierService.Delete(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("{id}/items")]
        public async Task<IActionResult> GetSupplierItems(int id)
        {
            var validationResult = ValidateApiKeyAndUser("all");
            if (validationResult != null)
            {
                return validationResult;
            }

            try
            {
                var targetSupplier = _supplierService.GetById(id);
                return Ok(targetSupplier);
            }
            catch (KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }
        }
    }
}