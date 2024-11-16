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

        public Task Create(Inventory entity)
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

        public List<string> AuditInventoryByLocation(Dictionary<int, Dictionary<int, int>> physicalCountsByLocation)
        {
            var inventories = GetAll();
            var discrepancies = new List<string>();

            foreach (var inventoryEntry in physicalCountsByLocation)
            {
                var inventory = inventories.FirstOrDefault(i => i.Id == inventoryEntry.Key);
                if (inventory == null)
                {
                    discrepancies.Add($"Inventory ID {inventoryEntry.Key} not found.");
                    LogDiscrepancy(inventoryEntry.Key, "N/A", "Inventory not found", null, null);
                    continue;
                }

                // Aggregate physical counts for this inventory ID
                int totalPhysicalCount = inventoryEntry.Value.Values.Sum();

                if (inventory.Total_On_Hand != totalPhysicalCount)
                {
                    discrepancies.Add($"Mismatch for ID {inventory.Id}: System Total = {inventory.Total_On_Hand}, Physical Total = {totalPhysicalCount}");
                    LogDiscrepancy(inventory.Id, inventory.Total_On_Hand, totalPhysicalCount, "Total mismatch", inventory.Locations.FirstOrDefault());
                }

                // Check each location
                foreach (var locationEntry in inventoryEntry.Value)
                {
                    int locationId = locationEntry.Key;
                    int physicalCount = locationEntry.Value;

                    if (!inventory.Locations.Contains(locationId))
                    {
                        discrepancies.Add($"Inventory ID {inventory.Id} does not exist in Location {locationId}.");
                        LogDiscrepancy(inventory.Id, "N/A", physicalCount, "Invalid location", locationId);
                        continue;
                    }
                }
            }

            return discrepancies;
        }

        private void LogDiscrepancy(int inventoryId, object expected, object actual, string reason, int? locationId)
        {
            var logEntry = new
            {
                InventoryId = inventoryId,
                Expected = expected,
                Actual = actual,
                Reason = reason,
                LocationId = locationId,
                Timestamp = DateTime.UtcNow
            };

            var logDir = "logs";
            var logFilePath = Path.Combine(logDir, "inventory_audit_logs.json");
            Directory.CreateDirectory(logDir);

            var logs = new List<object>();
            if (File.Exists(logFilePath))
            {
                var jsonData = File.ReadAllText(logFilePath);
                logs = JsonConvert.DeserializeObject<List<object>>(jsonData) ?? new List<object>();
            }

            logs.Add(logEntry);
            File.WriteAllText(logFilePath, JsonConvert.SerializeObject(logs, Formatting.Indented));
        }
    }
}