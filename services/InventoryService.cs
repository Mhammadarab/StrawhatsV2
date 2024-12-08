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

        public List<Inventory> GetAll(int? pageNumber = null, int? pageSize = null)
        {
            var jsonData = File.ReadAllText(jsonFilePath);
            var inventories = JsonConvert.DeserializeObject<List<Inventory>>(jsonData) ?? new List<Inventory>();

            // Apply pagination only if both pageNumber and pageSize are provided
            if (pageNumber.HasValue && pageSize.HasValue && pageNumber > 0 && pageSize > 0)
            {
                inventories = inventories
                    .Skip((pageNumber.Value - 1) * pageSize.Value)
                    .Take(pageSize.Value)
                    .ToList();
            }

            return inventories;
        }

        public Inventory GetById(int id)
        {
            var inventories = GetAll();
            return inventories.FirstOrDefault(i => i.Id == id);
        }

        public Task Update(Inventory entity)
        {
            var inventories = GetAll();
            var inventory = inventories.FirstOrDefault(i => i.Id == entity.Id);

            if (inventory == null)
            {
                throw new KeyNotFoundException($"Inventory with ID {entity.Id} not found.");
            }

            inventory.Item_Id = entity.Item_Id;
            inventory.Description = entity.Description;
            inventory.Item_Reference = entity.Item_Reference;
            inventory.Locations = entity.Locations;
            inventory.Total_On_Hand = entity.Total_On_Hand;
            inventory.Total_Expected = entity.Total_Expected;
            inventory.Total_Ordered = entity.Total_Ordered;
            inventory.Total_Allocated = entity.Total_Allocated;
            inventory.Total_Available = entity.Total_Available;
            inventory.Updated_At = DateTime.UtcNow;

            SaveToFile(inventories);
            return Task.CompletedTask;
        }

        public List<string> AuditInventory(string performedBy, Dictionary<int, Dictionary<int, int>> physicalCountsByLocation)
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
                            // Update the stock based on the physical count
                            inventory.Locations[locationId.ToString()] = physicalCount;
                        }
                    }
                    else
                    {
                        discrepancies.Add(
                            $"Location {locationId} not found for Inventory ID {inventory.Id}."
                        );
                    }
                }
            }

            // Save the updated inventories
            SaveToFile(inventories);

            // Log the discrepancies
            LogAuditChange(performedBy, physicalCountsByLocation, discrepancies);
            return discrepancies;
        }

        private void LogAuditChange(string performedBy, Dictionary<int, Dictionary<int, int>> auditData, List<string> discrepancies)
        {
            var convertedAuditData = auditData.ToDictionary(
                kvp => kvp.Key.ToString(),
                kvp => kvp.Value.ToDictionary(innerKvp => innerKvp.Key.ToString(), innerKvp => innerKvp.Value)
            );

            var logEntry = new LogEntry
            {
                Timestamp = DateTime.UtcNow.ToString("o"),
                PerformedBy = performedBy,
                AuditData = convertedAuditData,
                Discrepancies = discrepancies
            };

            var logFilePath = Path.Combine("logs", "inventory_audit.json");
            Directory.CreateDirectory("logs");

            List<LogEntry> logs;
            if (File.Exists(logFilePath))
            {
                var jsonData = File.ReadAllText(logFilePath);
                logs = JsonConvert.DeserializeObject<List<LogEntry>>(jsonData) ?? new List<LogEntry>();
            }
            else
            {
                logs = new List<LogEntry>();
            }

            logs.Add(logEntry);
            File.WriteAllText(logFilePath, JsonConvert.SerializeObject(logs, Formatting.Indented));
        }

        private void SaveToFile(List<Inventory> inventories)
        {
            var jsonData = JsonConvert.SerializeObject(inventories, Formatting.Indented);
            File.WriteAllText(jsonFilePath, jsonData);
        }
    }
}