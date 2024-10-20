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

        public async Task Create(ItemLine entity)
        {
            var itemLines = GetAll() ?? new List<ItemLine>();

            var nextId = itemLines.Any() ? itemLines.Max(il => il.Id) + 1 : 1;
            entity.Id = nextId;
            entity.CreatedAt = DateTime.Now;
            entity.UpdatedAt = DateTime.Now;

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
            existingItemLine.CreatedAt = entity.CreatedAt;
            existingItemLine.UpdatedAt = DateTime.Now;

            await SaveToFile(itemLines);
        }

        private async Task SaveToFile(List<ItemLine> itemLines)
        {
            var jsonData = JsonConvert.SerializeObject(itemLines, Formatting.Indented);
            await File.WriteAllTextAsync(jsonFilePath, jsonData);
        }
    }
}