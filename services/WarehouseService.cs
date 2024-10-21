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
    public class WarehouseService : ICrudService<Warehouse, int>
    {
        private readonly string jsonFilePath = "data/warehouses.json";

        public Task Create(Warehouse entity)
        {
            var warehouses = GetAll() ?? new List<Warehouse>();

            // Find the next available ID
            var nextId = warehouses.Any() ? warehouses.Max(w => w.Id) + 1 : 1;
            entity.Id = nextId;

            warehouses.Add(entity);
            SaveToFile(warehouses);
            return Task.CompletedTask;
        }

        public Task Delete(int id)
        {
            var warehouses = GetAll() ?? new List<Warehouse>();
            var warehouse = warehouses.FirstOrDefault(w => w.Id == id);

            if (warehouse == null)
            {
                throw new KeyNotFoundException($"Warehouse with ID {id} not found.");
            }

            warehouses.Remove(warehouse);
            SaveToFile(warehouses);
            return Task.CompletedTask;
        }

        public List<Warehouse> GetAll()
        {
            var jsonData = File.ReadAllText(jsonFilePath);
            return JsonConvert.DeserializeObject<List<Warehouse>>(jsonData) ?? new List<Warehouse>();
        }

        public Warehouse GetById(int id)
        {
            var warehouses = GetAll();
            var warehouse = warehouses.FirstOrDefault(w => w.Id == id);

            if (warehouse == null)
            {
                throw new KeyNotFoundException($"Warehouse with ID {id} not found.");
            }

            return warehouse;
        }

        public Task Update(Warehouse entity)
        {
            var warehouses = GetAll() ?? new List<Warehouse>();
            var existingWarehouse = warehouses.FirstOrDefault(w => w.Id == entity.Id);

            if (existingWarehouse == null)
            {
                throw new KeyNotFoundException($"Warehouse with ID {entity.Id} not found.");
            }

            // Update the properties
            existingWarehouse.Code = entity.Code;
            existingWarehouse.Name = entity.Name;
            existingWarehouse.Address = entity.Address;
            existingWarehouse.Zip = entity.Zip;
            existingWarehouse.City = entity.City;
            existingWarehouse.Province = entity.Province;
            existingWarehouse.Country = entity.Country;
            existingWarehouse.Contact = entity.Contact;
            existingWarehouse.Created_At = entity.Created_At;
            existingWarehouse.Updated_At = entity.Updated_At;

            SaveToFile(warehouses);
            return Task.CompletedTask;
        }

        private void SaveToFile(List<Warehouse> warehouses)
        {
            var jsonData = JsonConvert.SerializeObject(warehouses, Formatting.Indented);
            File.WriteAllText(jsonFilePath, jsonData);
        }
    }
}
