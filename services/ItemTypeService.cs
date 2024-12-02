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
        private readonly string itemsFilePath = "data/items.json";

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

        public List<ItemType> GetAll(int? pageNumber = null, int? pageSize = null)
        {
            if (!File.Exists(jsonFilePath))
            {
                return new List<ItemType>();
            }

            var jsonData = File.ReadAllText(jsonFilePath);
            var itemTypes = JsonConvert.DeserializeObject<List<ItemType>>(jsonData) ?? new List<ItemType>();

            // Apply pagination only if pageNumber and pageSize are provided and valid
            if (pageNumber.HasValue && pageSize.HasValue && pageNumber > 0 && pageSize > 0)
            {
                itemTypes = itemTypes
                    .Skip((pageNumber.Value - 1) * pageSize.Value)
                    .Take(pageSize.Value)
                    .ToList();
            }

            return itemTypes;
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

        public List<Item> GetItemsByItemTypeId(int itemTypeId)
        {
            if (!File.Exists(itemsFilePath))
            {
                return new List<Item>();
            }

            var jsonData = File.ReadAllText(itemsFilePath);
            var items = JsonConvert.DeserializeObject<List<Item>>(jsonData) ?? new List<Item>();
            return items.Where(it => it.ItemType == itemTypeId).ToList();
        }

        private async Task SaveToFile(List<ItemType> itemTypes)
        {
            var jsonData = JsonConvert.SerializeObject(itemTypes, Formatting.Indented);
            await File.WriteAllTextAsync(jsonFilePath, jsonData);
        }
    }
}