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
        private readonly string locationsFilePath = "data/locations.json";
        private readonly string inventoriesFilePath = "data/inventories.json";

        

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

        public List<Location> GetWarehouseLocations(int warehouseId)
        {
            var jsonData = File.ReadAllText(locationsFilePath);
            var allLocations = JsonConvert.DeserializeObject<List<Location>>(jsonData) ?? new List<Location>();

            return allLocations.Where(location => location.Warehouse_Id == warehouseId).ToList();
        }

        private List<Inventory> GetInventories()
        {
            var jsonData = File.ReadAllText(inventoriesFilePath);
            return JsonConvert.DeserializeObject<List<Inventory>>(jsonData) ?? new List<Inventory>();
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

        public List<Warehouse> GetAll(int? pageNumber = null, int? pageSize = null)
        {
            var jsonData = File.ReadAllText(jsonFilePath);
            var warehouses = JsonConvert.DeserializeObject<List<Warehouse>>(jsonData) ?? new List<Warehouse>();

            // Apply pagination only if pageNumber and pageSize are provided and valid
            if (pageNumber.HasValue && pageSize.HasValue && pageNumber > 0 && pageSize > 0)
            {
                warehouses = warehouses
                    .Skip((pageNumber.Value - 1) * pageSize.Value)
                    .Take(pageSize.Value)
                    .ToList();
            }

            return warehouses;
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

         public (int totalCapacity, int currentCapacity) CalculateWarehouseCapacities(int warehouseId)
        {
            var inventories = GetInventories();
            var warehouseLocations = GetWarehouseLocations(warehouseId);

            if (!warehouseLocations.Any())
                throw new KeyNotFoundException($"No locations found for Warehouse ID {warehouseId}");

            int totalCapacity = 0;
            int currentCapacity = 0;

            foreach (var inventory in inventories)
            {
                foreach (var location in inventory.Locations)
                {
                    // Check if the location belongs to the warehouse
                    if (warehouseLocations.Any(loc => loc.Id == Convert.ToInt32(location.Key)))
                    {
                        totalCapacity += location.Value; // Capacity per location
                        currentCapacity += location.Value; // Adjust based on utilization logic if needed
                    }
                }
            }

            return (totalCapacity, currentCapacity);
        }

        public List<object> CalculateAllWarehouseCapacities(int pageNumber, int pageSize)
        {
            var warehouses = GetAll();
            var pagedWarehouses = warehouses
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var capacities = new List<object>();

            foreach (var warehouse in pagedWarehouses)
            {
                var (totalCapacity, currentCapacity) = CalculateWarehouseCapacities(warehouse.Id);
                capacities.Add(new
                {
                    WarehouseId = warehouse.Id,
                    WarehouseName = warehouse.Name,
                    TotalCapacity = totalCapacity,
                    CurrentCapacity = currentCapacity
                });
            }

            return capacities;
        }


        private void SaveToFile(List<Warehouse> warehouses)
        {
            var jsonData = JsonConvert.SerializeObject(warehouses, Formatting.Indented);
            File.WriteAllText(jsonFilePath, jsonData);
        }
    }
}
