using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cargohub.interfaces;
using Cargohub.models;
using Microsoft.AspNetCore.Mvc;

namespace Cargohub.controllers
{
    [ApiExplorerSettings(GroupName = "Suppliers")]
    [Route("api/v1/suppliers/")]
    [ApiController]
    public class SupplierController : Controller
    {
        private readonly ICrudService<Supplier, int> _supplierService;

        public SupplierController(ICrudService<Supplier, int> supplierService)
        {
            _supplierService = supplierService;
        }

        [HttpGet]
        public IActionResult GetSuppliers()
        {
            var suppliers = _supplierService.GetAll();
            if (suppliers == null || !suppliers.Any())
            {
                return NotFound();
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