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
    public class ShipmentService : ICrudService<Shipment, int>
    {
        private readonly string jsonFilePath = "data/shipments.json";

        public Task Create(Shipment entity)
        {
            var shipments = GetAll() ?? new List<Shipment>();

            // Find the next available ID
            var nextId = shipments.Any() ? shipments.Max(s => s.Id) + 1 : 1;
            entity.Id = nextId;
            entity.Created_At = DateTime.Now;
            entity.Updated_At = DateTime.Now;
            shipments.Add(entity);
            SaveToFile(shipments);
            return Task.CompletedTask;
        }

        public Task Delete(int id)
        {
            var shipments = GetAll() ?? new List<Shipment>();
            var shipment = shipments.FirstOrDefault(s => s.Id == id);

            if (shipment == null)
            {
                throw new KeyNotFoundException($"Shipment with ID {id} not found.");
            }

            shipments.Remove(shipment);
            SaveToFile(shipments);
            return Task.CompletedTask;
        }

        public List<Shipment> GetAll()
        {
            var jsonData = File.ReadAllText(jsonFilePath);
            return JsonConvert.DeserializeObject<List<Shipment>>(jsonData) ?? new List<Shipment>();
        }

        public Shipment GetById(int id)
        {
            var shipments = GetAll();
            var shipment = shipments.FirstOrDefault(s => s.Id == id);

            if (shipment == null)
            {
                throw new KeyNotFoundException($"Shipment with ID {id} not found.");
            }

            return shipment;
        }

        public Task Update(Shipment entity)
        {
            var shipments = GetAll() ?? new List<Shipment>();
            var existingShipment = shipments.FirstOrDefault(s => s.Id == entity.Id);

            if (existingShipment == null)
            {
                throw new KeyNotFoundException($"Shipment with ID {entity.Id} not found.");
            }

            existingShipment.Order_Id = entity.Order_Id;
            existingShipment.Source_Id = entity.Order_Id;
            existingShipment.Order_Date = entity.Order_Date;
            existingShipment.Request_Date = entity.Request_Date;
            existingShipment.Shipment_Date = entity.Shipment_Date;
            existingShipment.Shipment_Type = entity.Shipment_Type;
            existingShipment.Shipment_Status = entity.Shipment_Status;
            existingShipment.Notes = entity.Notes;
            existingShipment.Carrier_Code = entity.Carrier_Code;
            existingShipment.Carrier_Description = entity.Carrier_Description;
            existingShipment.Service_Code = entity.Service_Code;
            existingShipment.Payment_Type = entity.Payment_Type;
            existingShipment.Transfer_Mode = entity.Transfer_Mode;
            existingShipment.Total_Package_Count = entity.Total_Package_Count;
            existingShipment.Total_Package_Weight = entity.Total_Package_Weight;
            existingShipment.Items = entity.Items;


            
            existingShipment.Updated_At = DateTime.Now;
            SaveToFile(shipments);
            return Task.CompletedTask;
        }

        private void SaveToFile(List<Shipment> shipments)
        {
            var jsonData = JsonConvert.SerializeObject(shipments, Formatting.Indented);
            File.WriteAllText(jsonFilePath, jsonData);
        }
    }
}