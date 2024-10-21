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
    public class ItemLineService : ICrudService<ItemLine, int>
    {
        private readonly string jsonFilePath = "data/item_lines.json";
        private readonly string itemsFilePath = "data/items.json";

        public async Task Create(ItemLine entity)
        {
            var itemLines = GetAll() ?? new List<ItemLine>();

            var nextId = itemLines.Any() ? itemLines.Max(il => il.Id) + 1 : 1;
            entity.Id = nextId;
            entity.Created_At = DateTime.Now;
            entity.Updated_At = DateTime.Now;

            itemLines.Add(entity);
            await SaveToFile(itemLines);
        }

        public async Task Delete(int id)
        {
            var itemLines = GetAll() ?? new List<ItemLine>();
            var itemLine = itemLines.FirstOrDefault(il => il.Id == id);

            if (itemLine == null)
            {
                throw new KeyNotFoundException($"ItemLine with ID {id} not found.");
            }

            itemLines.Remove(itemLine);
            await SaveToFile(itemLines);
        }

        public List<ItemLine> GetAll()
        {
            if (!File.Exists(jsonFilePath))
            {
                return new List<ItemLine>();
            }

            var jsonData = File.ReadAllText(jsonFilePath);
            return JsonConvert.DeserializeObject<List<ItemLine>>(jsonData) ?? new List<ItemLine>();
        }

        public ItemLine GetById(int id)
        {
            var itemLines = GetAll();
            var itemLine = itemLines.FirstOrDefault(il => il.Id == id);

            if (itemLine == null)
            {
                throw new KeyNotFoundException($"ItemLine with ID {id} not found.");
            }

            return itemLine;
        }

        public async Task Update(ItemLine entity)
        {
            var itemLines = GetAll() ?? new List<ItemLine>();
            var existingItemLine = itemLines.FirstOrDefault(il => il.Id == entity.Id);

            if (existingItemLine == null)
            {
                throw new KeyNotFoundException($"ItemLine with ID {entity.Id} not found.");
            }

            existingItemLine.Name = entity.Name;
            existingItemLine.Description = entity.Description;
            existingItemLine.Updated_At = DateTime.Now;

            await SaveToFile(itemLines);
        }

        public List<Item> GetItemsByItemLineId(int itemLineId)
        {
            if (!File.Exists(itemsFilePath))
            {
                return new List<Item>();
            }

            var jsonData = File.ReadAllText(itemsFilePath);
            var items = JsonConvert.DeserializeObject<List<Item>>(jsonData) ?? new List<Item>();
            return items.Where(it => it.ItemLine == itemLineId).ToList();
        }

        private async Task SaveToFile(List<ItemLine> itemLines)
        {
            var jsonData = JsonConvert.SerializeObject(itemLines, Formatting.Indented);
            await File.WriteAllTextAsync(jsonFilePath, jsonData);
        }
    }
}