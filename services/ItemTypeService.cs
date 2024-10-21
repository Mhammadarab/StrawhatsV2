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
    public class ItemTypeService : ICrudService<ItemType, int>
    {
        private readonly string jsonFilePath = "data/item_types.json";

        public async Task Create(ItemType entity)
        {
            var itemTypes = GetAll() ?? new List<ItemType>();

            var nextId = itemTypes.Any() ? itemTypes.Max(it => it.Id) + 1 : 1;
            entity.Id = nextId;
            entity.Created_At = DateTime.Now;
            entity.Updated_At = DateTime.Now;

            itemTypes.Add(entity);
            await SaveToFile(itemTypes);
        }

        public async Task Delete(int id)
        {
            var itemTypes = GetAll() ?? new List<ItemType>();
            var itemType = itemTypes.FirstOrDefault(it => it.Id == id);

            if (itemType == null)
            {
                throw new KeyNotFoundException($"ItemType with ID {id} not found.");
            }

            itemTypes.Remove(itemType);
            await SaveToFile(itemTypes);
        }

        public List<ItemType> GetAll()
        {
            if (!File.Exists(jsonFilePath))
            {
                return new List<ItemType>();
            }

            var jsonData = File.ReadAllText(jsonFilePath);
            return JsonConvert.DeserializeObject<List<ItemType>>(jsonData) ?? new List<ItemType>();
        }

        public ItemType GetById(int id)
        {
            var itemTypes = GetAll();
            var itemType = itemTypes.FirstOrDefault(it => it.Id == id);

            if (itemType == null)
            {
                throw new KeyNotFoundException($"ItemType with ID {id} not found.");
            }

            return itemType;
        }

        public async Task Update(ItemType entity)
        {
            var itemTypes = GetAll() ?? new List<ItemType>();
            var existingItemType = itemTypes.FirstOrDefault(it => it.Id == entity.Id);

            if (existingItemType == null)
            {
                throw new KeyNotFoundException($"ItemType with ID {entity.Id} not found.");
            }

            existingItemType.Name = entity.Name;
            existingItemType.Description = entity.Description;
            existingItemType.Updated_At = DateTime.Now;

            await SaveToFile(itemTypes);
        }

        private async Task SaveToFile(List<ItemType> itemTypes)
        {
            var jsonData = JsonConvert.SerializeObject(itemTypes, Formatting.Indented);
            await File.WriteAllTextAsync(jsonFilePath, jsonData);
        }
    }
}