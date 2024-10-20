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
    public class OrderService : ICrudService<Order, int>
    {
        private readonly string jsonFilePath = "data/orders.json";

        public Task Create(Order entity)
        {
            var orders = GetAll() ?? new List<Order>();

            // Find the next available ID
            var nextId = orders.Any() ? orders.Max(o => o.Id) + 1 : 1;
            entity.Id = nextId;

            orders.Add(entity);
            SaveToFile(orders);
            return Task.CompletedTask;
        }

        public Task Delete(int id)
        {
            var orders = GetAll() ?? new List<Order>();
            var order = orders.FirstOrDefault(o => o.Id == id);

            if (order == null)
            {
                throw new KeyNotFoundException($"Order with ID {id} not found.");
            }

            orders.Remove(order);
            SaveToFile(orders);
            return Task.CompletedTask;
        }

        public List<Order> GetAll()
        {
            var jsonData = File.ReadAllText(jsonFilePath);
            return JsonConvert.DeserializeObject<List<Order>>(jsonData) ?? new List<Order>();
        }

        public Order GetById(int id)
        {
            var orders = GetAll();
            var order = orders.FirstOrDefault(o => o.Id == id);

            if (order == null)
            {
                throw new KeyNotFoundException($"Order with ID {id} not found.");
            }

            return order;
        }

        public Task Update(Order entity)
        {
            var orders = GetAll() ?? new List<Order>();
            var existingOrder = orders.FirstOrDefault(o => o.Id == entity.Id);

            if (existingOrder == null)
            {
                throw new KeyNotFoundException($"Order with ID {entity.Id} not found.");
            }

            existingOrder.SourceId = entity.SourceId;
            existingOrder.OrderDate = entity.OrderDate;
            existingOrder.RequestDate = entity.RequestDate;
            existingOrder.Reference = entity.Reference;
            existingOrder.ReferenceExtra = entity.ReferenceExtra;
            existingOrder.OrderStatus = entity.OrderStatus;
            existingOrder.Notes = entity.Notes;
            existingOrder.ShippingNotes = entity.ShippingNotes;
            existingOrder.PickingNotes= entity.PickingNotes;
            existingOrder.WarehouseId = entity.WarehouseId;
            existingOrder.ShipTo = entity.ShipTo;
            existingOrder.BillTo = entity.BillTo;
            existingOrder.ShipmentId = entity.ShipmentId;
            existingOrder.TotalAmount = entity.TotalAmount;
            existingOrder.TotalDiscount = entity.TotalDiscount;
            existingOrder.TotalTax = entity.TotalTax;
            existingOrder.TotalSurcharge = entity.TotalSurcharge;
            existingOrder.CreatedAt = entity.CreatedAt;
            existingOrder.UpdatedAt = entity.UpdatedAt;
            existingOrder.Items = entity.Items;
            // Update other fields as necessary

            SaveToFile(orders);
            return Task.CompletedTask;
        }

        private void SaveToFile(List<Order> orders)
        {
            var jsonData = JsonConvert.SerializeObject(orders, Formatting.Indented);
            File.WriteAllText(jsonFilePath, jsonData);
        }
    }
}