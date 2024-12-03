using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cargohub.interfaces;
using Cargohub.models;
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

        [HttpGet]
        public IActionResult GetSuppliers([FromQuery] int? pageNumber = null, [FromQuery] int? pageSize = null)
        {
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
        public async Task<IActionResult> GetSupplierItems(int id) {
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