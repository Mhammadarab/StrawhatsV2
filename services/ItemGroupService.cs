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
    public class ItemGroupService : ICrudService<ItemGroup, int>
    {
        private readonly string jsonFilePath = "data/item_groups.json";

        public Task Create(ItemGroup entity)
        {
            var itemGroups = GetAll() ?? new List<ItemGroup>();

            // Find the next available ID
            var nextId = itemGroups.Any() ? itemGroups.Max(ig => ig.Id) + 1 : 1;
            entity.Id = nextId;
            entity.Created_At = DateTime.UtcNow;
            entity.Updated_At = DateTime.UtcNow;

            itemGroups.Add(entity);
            SaveToFile(itemGroups);
            return Task.CompletedTask;
        }

        public Task Delete(int id)
        {
            var itemGroups = GetAll() ?? new List<ItemGroup>();
            var itemGroup = itemGroups.FirstOrDefault(ig => ig.Id == id);

            if (itemGroup == null)
            {
                throw new KeyNotFoundException($"ItemGroup with ID {id} not found.");
            }

            itemGroups.Remove(itemGroup);
            SaveToFile(itemGroups);
            return Task.CompletedTask;
        }

        public List<ItemGroup> GetAll(int? pageNumber = null, int? pageSize = null)
        {
            if (!File.Exists(jsonFilePath))
            {
                return new List<ItemGroup>();
            }

            var jsonData = File.ReadAllText(jsonFilePath);
            var itemGroups = JsonConvert.DeserializeObject<List<ItemGroup>>(jsonData) ?? new List<ItemGroup>();

            // Apply pagination only if pageNumber and pageSize are provided and valid
            if (pageNumber.HasValue && pageSize.HasValue && pageNumber > 0 && pageSize > 0)
            {
                itemGroups = itemGroups
                    .Skip((pageNumber.Value - 1) * pageSize.Value)
                    .Take(pageSize.Value)
                    .ToList();
            }

            return itemGroups;
        }


        public ItemGroup GetById(int id)
        {
            var itemGroups = GetAll();
            var itemGroup = itemGroups.FirstOrDefault(ig => ig.Id == id);

            if (itemGroup == null)
            {
                throw new KeyNotFoundException($"ItemGroup with ID {id} not found.");
            }

            return itemGroup;
        }

        public Task Update(ItemGroup entity)
        {
            var itemGroups = GetAll() ?? new List<ItemGroup>();
            var existingItemGroup = itemGroups.FirstOrDefault(ig => ig.Id == entity.Id);

            if (existingItemGroup == null)
            {
                throw new KeyNotFoundException($"ItemGroup with ID {entity.Id} not found.");
            }

            // Update properties
            existingItemGroup.Name = entity.Name;
            existingItemGroup.Description = entity.Description;
            existingItemGroup.Updated_At = DateTime.UtcNow;

            SaveToFile(itemGroups);
            return Task.CompletedTask;
        }

        private void SaveToFile(List<ItemGroup> itemGroups)
        {
            var jsonData = JsonConvert.SerializeObject(itemGroups, Formatting.Indented);
            File.WriteAllText(jsonFilePath, jsonData);
        }
    }
}
