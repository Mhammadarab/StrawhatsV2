using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Cargohub.interfaces;
using Cargohub.models;
using Newtonsoft.Json;

namespace Cargohub.services
{
    public class SupplierService : ICrudService<Supplier, int>
    {
        private readonly string jsonFilePath = "data/suppliers.json";

        public Task Create(Supplier entity)
        {
            var suppliers = GetAll() ?? new List<Supplier>();

            // Find the next available ID
            var nextId = suppliers.Any() ? suppliers.Max(s => s.Id) + 1 : 1;
            entity.Id = nextId;
            entity.Created_At = DateTime.Now;
            entity.Updated_At = DateTime.Now;
            suppliers.Add(entity);
            SaveToFile(suppliers);
            return Task.CompletedTask;
        }

        public Task Delete(int id)
        {
            var suppliers = GetAll() ?? new List<Supplier>();
            var supplier = suppliers.FirstOrDefault(s => s.Id == id);

            if (supplier == null)
            {
                throw new KeyNotFoundException($"Supplier with ID {id} not found.");
            }

            suppliers.Remove(supplier);
            SaveToFile(suppliers);
            return Task.CompletedTask;
        }

        public List<Supplier> GetAll(int? pageNumber = null, int? pageSize = null)
        {
            var jsonData = File.ReadAllText(jsonFilePath);
            var suppliers = JsonConvert.DeserializeObject<List<Supplier>>(jsonData) ?? new List<Supplier>();

            // Apply pagination only if pageNumber and pageSize are provided and valid
            if (pageNumber.HasValue && pageSize.HasValue && pageNumber > 0 && pageSize > 0)
            {
                suppliers = suppliers
                    .Skip((pageNumber.Value - 1) * pageSize.Value)
                    .Take(pageSize.Value)
                    .ToList();
            }

            return suppliers;
        }

        public Supplier GetById(int id)
        {
            var suppliers = GetAll();
            var supplier = suppliers.FirstOrDefault(s => s.Id == id);

            if (supplier == null)
            {
                throw new KeyNotFoundException($"Supplier with ID {id} not found.");
            }

            return supplier;
        }

        public Task Update(Supplier entity)
        {
            var suppliers = GetAll() ?? new List<Supplier>();
            var existingSupplier = suppliers.FirstOrDefault(s => s.Id == entity.Id);

            if (existingSupplier == null)
            {
                throw new KeyNotFoundException($"Supplier with ID {entity.Id} not found.");
            }
            existingSupplier.Code = entity.Code;
            existingSupplier.Name = entity.Name;
            existingSupplier.Address = entity.Address;
            existingSupplier.Address_Extra = entity.Address_Extra;
            existingSupplier.City = entity.City;
            existingSupplier.Zip_Code = entity.Zip_Code;
            existingSupplier.Province = entity.Province;
            existingSupplier.Country = entity.Country;
            existingSupplier.Contact_Name = entity.Contact_Name;
            existingSupplier.PhoneNumber = entity.PhoneNumber;
            existingSupplier.Reference = entity.Reference;

            existingSupplier.Updated_At = DateTime.Now;
            SaveToFile(suppliers);
            return Task.CompletedTask;
        }

        private void SaveToFile(List<Supplier> suppliers)
        {
            var jsonData = JsonConvert.SerializeObject(suppliers, Formatting.Indented);
            File.WriteAllText(jsonFilePath, jsonData);
        }
    }
}