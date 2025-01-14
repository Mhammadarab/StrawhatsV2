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
        private readonly ShipmentService _shipmentService;

        public OrderService(ShipmentService shipmentService)
        {
            _shipmentService = shipmentService;
        }
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

        public async Task UpdateBackorderStatus(int orderId)
        {
            var orders = GetAll() ?? new List<Order>();
            var order = orders.FirstOrDefault(o => o.Id == orderId);

            if (order == null)
            {
                throw new KeyNotFoundException($"Order with ID {orderId} not found.");
            }

            var shipments = _shipmentService.GetAll();
            var orderShipments = shipments.Where(s => s.Order_Id.Contains(orderId)).ToList();

            var missingItems = new List<ItemDetail>();
            foreach (var item in order.Items)
            {
                var totalShippedAmount = orderShipments
                    .SelectMany(s => s.Items)
                    .Where(si => si.Item_Id == item.Item_Id)
                    .Sum(si => si.Amount);

                if (totalShippedAmount < item.Amount)
                {
                    missingItems.Add(new ItemDetail
                    {
                        Item_Id = item.Item_Id,
                        Amount = item.Amount - totalShippedAmount,
                        CrossDockingStatus = null
                    });
                }
            }

            if (!missingItems.Any())
            {
                throw new InvalidOperationException("All items in the order match the items in the shipment. No items to backorder.");
            }

            order.IsBackordered = true;
            order.ShipmentDetails = missingItems;

            // Remove the missing items from the order's Items list to avoid duplicates
            order.Items = order.Items.Where(item => !missingItems.Any(mi => mi.Item_Id == item.Item_Id)).ToList();

            await Update(order);
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

        public List<Order> GetAll(int? pageNumber = null, int? pageSize = null)
        {
            var jsonData = File.ReadAllText(jsonFilePath);
            var orders = JsonConvert.DeserializeObject<List<Order>>(jsonData) ?? new List<Order>();

            // Apply pagination only if pageNumber and pageSize are provided and valid
            if (pageNumber.HasValue && pageSize.HasValue && pageNumber > 0 && pageSize > 0)
            {
                orders = orders
                    .Skip((pageNumber.Value - 1) * pageSize.Value)
                    .Take(pageSize.Value)
                    .ToList();
            }

            return orders;
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

            existingOrder.Source_Id = entity.Source_Id;
            existingOrder.Order_Date = entity.Order_Date;
            existingOrder.Request_Date = entity.Request_Date;
            existingOrder.Reference = entity.Reference;
            existingOrder.Reference_Extra = entity.Reference_Extra;
            existingOrder.Order_Status = entity.Order_Status;
            existingOrder.Notes = entity.Notes;
            existingOrder.Shipping_Notes = entity.Shipping_Notes;
            existingOrder.Picking_Notes = entity.Picking_Notes;
            existingOrder.Warehouse_Id = entity.Warehouse_Id;
            existingOrder.Ship_To = entity.Ship_To;
            existingOrder.Bill_To = entity.Bill_To;
            existingOrder.Shipment_Id = entity.Shipment_Id;
            existingOrder.Total_Amount = entity.Total_Amount;
            existingOrder.Total_Discount = entity.Total_Discount;
            existingOrder.Total_Tax = entity.Total_Tax;
            existingOrder.Total_Surcharge = entity.Total_Surcharge;
            existingOrder.Created_At = entity.Created_At;
            existingOrder.Updated_At = DateTime.Now;
            existingOrder.Items = entity.Items;
            existingOrder.IsBackordered = entity.IsBackordered; // Update backorder status
            existingOrder.ShipmentDetails = entity.ShipmentDetails; // Update shipment details

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