using Cargohub.interfaces;
using Cargohub.models;
using Newtonsoft.Json;

namespace Cargohub.services
{
    public class ItemService : ICrudService<Item, string>
    {
        private readonly string jsonFilePath = "data/items.json";
        private readonly string inventoriesFilePath = "data/inventories.json";

        public async Task Create(Item entity)
        {
            var items = GetAll() ?? new List<Item>();

            // entity.Uid = Guid.NewGuid().ToString();
            entity.Created_At = DateTime.Now;
            entity.Updated_At = DateTime.Now;

            items.Add(entity);
            await SaveToFile(jsonFilePath, items);
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
            await SaveToFile(jsonFilePath, items);
        }

        public List<Item> GetAll(int? pageNumber = null, int? pageSize = null)
        {
            if (!File.Exists(jsonFilePath))
            {
                return new List<Item>();
            }

            var jsonData = File.ReadAllText(jsonFilePath);
            var items = JsonConvert.DeserializeObject<List<Item>>(jsonData) ?? new List<Item>();

            // Apply pagination only if pageNumber and pageSize are provided and valid
            if (pageNumber.HasValue && pageSize.HasValue && pageNumber > 0 && pageSize > 0)
            {
                items = items
                    .Skip((pageNumber.Value - 1) * pageSize.Value)
                    .Take(pageSize.Value)
                    .ToList();
            }

            return items;
        }


        public Item? GetById(string uid)
        {
            var items = GetAll();
            var item = items.FirstOrDefault(it => it.Uid == uid);

            if (item == null)
            {
                return item;
            }

            // Get inventory totals from inventories.json
            var inventoryTotals = GetInventoryTotalsByItemId(uid);
            // item.InventoryTotals = inventoryTotals;

            return item;
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

            await SaveToFile(jsonFilePath, items);
        }

        public InventoryTotals GetItemInventoryTotals(string itemId)
        {
            return GetInventoryTotalsByItemId(itemId);
        }

        private InventoryTotals GetInventoryTotalsByItemId(string itemId)
        {
            if (!File.Exists(inventoriesFilePath))
            {
                return new InventoryTotals();
            }

            var jsonData = File.ReadAllText(inventoriesFilePath);
            var inventories = JsonConvert.DeserializeObject<List<Inventory>>(jsonData) ?? new List<Inventory>();

            var inventory = inventories.FirstOrDefault(inv => inv.Item_Id == itemId);

            if (inventory == null)
            {
                return new InventoryTotals();
            }

            return new InventoryTotals
            {
                TotalOnHand = inventory.Total_On_Hand,
                TotalExpected = inventory.Total_Expected,
                TotalOrdered = inventory.Total_Ordered,
                TotalAllocated = inventory.Total_Allocated,
                TotalAvailable = inventory.Total_Available
            };
        }
        public Item AddClassifications(string itemUid, List<int> newClassifications)
        {
            var items = GetAll() ?? new List<Item>();
            var item = items.FirstOrDefault(it => it.Uid == itemUid);

            if (item == null)
            {
                throw new KeyNotFoundException($"Item with UID {itemUid} not found.");
            }

            if (item.Classifications_Id == null)
            {
                item.Classifications_Id = new List<int>();
            }

            item.Classifications_Id.AddRange(newClassifications.Except(item.Classifications_Id));
            SaveToFile(jsonFilePath, items);

            return item;
        }

        private async Task SaveToFile<T>(string filePath, List<T> data)
        {
            var jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);
            await File.WriteAllTextAsync(filePath, jsonData);
        }
    }
}