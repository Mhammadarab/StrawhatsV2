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
    public class InventoryService : ICrudService<Inventory, int>
    {
        private readonly string jsonFilePath = "data/inventories.json";

        public Task Create (Inventory entity)
        {
            var inventories = GetAll() ?? new List<Inventory>();

            // Find the next available ID
            var nextId = inventories.Any() ? inventories.Max(i => i.Id) + 1 : 1;
            entity.Id = nextId;

            inventories.Add(entity);
            SaveToFile(inventories);
            return Task.CompletedTask;
        }

        public Task Delete(int id)
        {
            var inventories = GetAll() ?? new List<Inventory>();
            var inventory = inventories.FirstOrDefault(i => i.Id == id);

            if (inventory == null)
            {
                throw new KeyNotFoundException($"Inventory with ID {id} not found.");
            }

            inventories.Remove(inventory);
            SaveToFile(inventories);
            return Task.CompletedTask;
        }

        public List<Inventory> GetAll()
        {
            var jsonData = File.ReadAllText(jsonFilePath);
            return JsonConvert.DeserializeObject<List<Inventory>>(jsonData) ?? new List<Inventory>();
        }

        public Inventory GetById(int id)
        {
            var inventories = GetAll();
            var inventory = inventories.FirstOrDefault(i => i.Id == id);

            if (inventory == null)
            {
                throw new KeyNotFoundException($"Inventory with ID {id} not found.");
            }

            return inventory;
        }

        public Task Update(Inventory entity)
        {
            var inventories = GetAll();
            var inventory = inventories.FirstOrDefault(i => i.Id == entity.Id);

            if (inventory == null)
            {
                throw new KeyNotFoundException($"Inventory with ID {entity.Id} not found.");
            }

            inventory.Id = entity.Id;
            inventory.Item_Id = entity.Item_Id;
            inventory.Description = entity.Description;
            inventory.Item_Reference = entity.Item_Reference;
            inventory.Locations = entity.Locations;
            inventory.Total_On_Hand = entity.Total_On_Hand;
            inventory.Total_Expected = entity.Total_Expected;
            inventory.Total_Ordered = entity.Total_Ordered;
            inventory.Total_Allocated = entity.Total_Allocated;
            inventory.Total_Available = entity.Total_Available;
            inventory.Created_At = entity.Created_At;
            inventory.Updated_At = DateTime.UtcNow;





            SaveToFile(inventories);
            return Task.CompletedTask;
        }

        private void SaveToFile(List<Inventory> inventories)
        {
            var jsonData = JsonConvert.SerializeObject(inventories, Formatting.Indented);
            File.WriteAllText(jsonFilePath, jsonData);
        }
    }
}