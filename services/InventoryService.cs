using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Cargohub.interfaces;
using Cargohub.models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Cargohub.services
{
    public class InventoryService : ICrudService<Inventory, int>
    {
        private readonly string jsonFilePath = "data/inventories.json";

        public Task Create(Inventory entity)
        {
            var inventories = GetAll() ?? new List<Inventory>();

            // Find the next available ID
            var nextId = inventories.Any() ? inventories.Max(i => i.Id) + 1 : 1;
            entity.Id = nextId;

            // Ensure Locations is a valid dictionary
            if (entity.Locations == null)
            {
                entity.Locations = new Dictionary<string, int>();
            }
            else
            {
                // Convert string keys to ensure consistency (keys must be valid integers)
                var validatedLocations = new Dictionary<string, int>();
                foreach (var location in entity.Locations)
                {
                    if (int.TryParse(location.Key, out _))
                    {
                        validatedLocations[location.Key] = location.Value;
                    }
                    else
                    {
                        throw new ArgumentException($"Invalid location type key: {location.Key}. Keys must integers.");
                    }
                }
                entity.Locations = validatedLocations;
            }

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
            var inventories = JsonConvert.DeserializeObject<List<Inventory>>(jsonData);

            foreach (var inventory in inventories)
            {
                // Ensure Locations is always deserialized as a dictionary
                if (inventory.Locations == null)
                {
                    inventory.Locations = new Dictionary<string, int>();
                }
            }

            return inventories;
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

        public List<string> AuditInventory(Dictionary<int, Dictionary<int, int>> physicalCountsByLocation)
        {
            var inventories = GetAll();
            var discrepancies = new List<string>();

            foreach (var auditEntry in physicalCountsByLocation)
            {
                var inventory = inventories.FirstOrDefault(i => i.Id == auditEntry.Key);

                if (inventory == null)
                {
                    discrepancies.Add($"Inventory ID {auditEntry.Key} not found.");
                    continue;
                }

                int totalPhysicalCount = auditEntry.Value.Values.Sum();

                // Check and update discrepancies for locations
                foreach (var locationEntry in auditEntry.Value)
                {
                    int locationId = locationEntry.Key;
                    int physicalCount = locationEntry.Value;

                    if (inventory.Locations.ContainsKey(locationId.ToString()))
                    {
                        int systemCount = inventory.Locations[locationId.ToString()];
                        if (systemCount != physicalCount)
                        {
                            discrepancies.Add(
                                $"Discrepancy for Inventory ID {inventory.Id} at Location {locationId}: System = {systemCount}, Physical = {physicalCount}"
                            );

                            // Update to physical count
                            inventory.Locations[locationId.ToString()] = physicalCount;
                        }
                    }
                    else
                    {
                        discrepancies.Add(
                            $"Location {locationId} not found for Inventory ID {inventory.Id}. Adding new location."
                        );
                        inventory.Locations[locationId.ToString()] = physicalCount;
                    }
                }

                // Update total on hand
                inventory.Total_On_Hand = inventory.Locations.Values.Sum();
            }

            SaveToFile(inventories);
            return discrepancies;
        }

        private void SaveToFile(List<Inventory> inventories)
        {
            var jsonData = JsonConvert.SerializeObject(inventories, Formatting.Indented);
            File.WriteAllText(jsonFilePath, jsonData);
        }
    }
}