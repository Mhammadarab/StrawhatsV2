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
    public class ItemService : ICrudService<Item, string>
    {
        private readonly string jsonFilePath = "data/items.json";

        public async Task Create(Item entity)
        {
            var items = GetAll() ?? new List<Item>();

            entity.Uid = Guid.NewGuid().ToString();
            entity.Created_At = DateTime.Now;
            entity.Updated_At = DateTime.Now;

            items.Add(entity);
            await SaveToFile(items);
        }

        public async Task Delete(string uid)
        {
            var items = GetAll() ?? new List<Item>();
            var item = items.FirstOrDefault(it => it.Uid == uid);

            if (item == null)
            {
                throw new KeyNotFoundException($"Item with UID {uid} not found.");
            }

            items.Remove(item);
            await SaveToFile(items);
        }

        public List<Item> GetAll()
        {
            if (!File.Exists(jsonFilePath))
            {
                return new List<Item>();
            }

            var jsonData = File.ReadAllText(jsonFilePath);
            return JsonConvert.DeserializeObject<List<Item>>(jsonData) ?? new List<Item>();
        }

        public Item GetById(string uid)
        {
            var items = GetAll();
            var item = items.FirstOrDefault(it => it.Uid == uid);

            if (item == null)
            {
                throw new KeyNotFoundException($"Item with UID {uid} not found.");
            }

            return item;
        }

        public List<string> GetItemsForItemLine(int itemLineId)
        {
            var items = GetAll();
            return items.Where(it => it.ItemLine == itemLineId).Select(it => it.Uid).ToList();
        }

        public List<string> GetItemsForItemGroup(int itemGroupId)
        {
            var items = GetAll();
            return items.Where(it => it.ItemGroup == itemGroupId).Select(it => it.Uid).ToList();
        }

        public List<string> GetItemsForItemType(int itemTypeId)
        {
            var items = GetAll();
            return items.Where(it => it.ItemType == itemTypeId).Select(it => it.Uid).ToList();
        }

        public List<Item> GetItemsForSupplier(int supplierId)
        {
            var items = GetAll();
            return items.Where(it => it.SupplierId == supplierId).ToList();
        }

        public async Task Update(Item entity)
        {
            var items = GetAll() ?? new List<Item>();
            var existingItem = items.FirstOrDefault(it => it.Uid == entity.Uid);

            if (existingItem == null)
            {
                throw new KeyNotFoundException($"Item with UID {entity.Uid} not found.");
            }

            existingItem.Code = entity.Code;
            existingItem.Description = entity.Description;
            existingItem.ShortDescription = entity.ShortDescription;
            existingItem.UpcCode = entity.UpcCode;
            existingItem.ModelNumber = entity.ModelNumber;
            existingItem.CommodityCode = entity.CommodityCode;
            existingItem.ItemLine = entity.ItemLine;
            existingItem.ItemGroup = entity.ItemGroup;
            existingItem.ItemType = entity.ItemType;
            existingItem.UnitPurchaseQuantity = entity.UnitPurchaseQuantity;
            existingItem.UnitOrderQuantity = entity.UnitOrderQuantity;
            existingItem.PackOrderQuantity = entity.PackOrderQuantity;
            existingItem.SupplierId = entity.SupplierId;
            existingItem.SupplierCode = entity.SupplierCode;
            existingItem.SupplierPartNumber = entity.SupplierPartNumber;
            existingItem.Updated_At = DateTime.Now;

            await SaveToFile(items);
        }

        private async Task SaveToFile(List<Item> items)
        {
            var jsonData = JsonConvert.SerializeObject(items, Formatting.Indented);
            await File.WriteAllTextAsync(jsonFilePath, jsonData);
        }
    }
}