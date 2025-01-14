using Cargohub.interfaces;
using Cargohub.models;
using Newtonsoft.Json;
using StrawhatsV2.models;

public class ShipmentService : ICrudService<Shipment, int>
{
    private readonly string jsonFilePath = "data/shipments.json";
    private readonly string logFilePath = "logs/picking_logs.log";

    public Task Create(Shipment entity)
    {
        var shipments = GetAll() ?? new List<Shipment>();
        var nextId = shipments.Any() ? shipments.Max(s => s.Id) + 1 : 1;

        entity.Id = nextId;
        entity.Created_At = DateTime.Now;
        entity.Updated_At = DateTime.Now;
        shipments.Add(entity);
        SaveToFile(shipments);

        return Task.CompletedTask;
    }

    public async Task SavePickingList(int shipmentId, Dictionary<string, int> pickedItems, string performedBy, string description = null)
        {
            var shipments = GetAll() ?? new List<Shipment>();
            var shipment = shipments.FirstOrDefault(s => s.Id == shipmentId);

            if (shipment == null)
            {
                throw new KeyNotFoundException($"Shipment with ID {shipmentId} not found.");
            }

            var invalidItems = new List<string>();

            foreach (var pickedItem in pickedItems)
            {
                var shipmentItem = shipment.Items.FirstOrDefault(i => i.Item_Id == pickedItem.Key);
                if (shipmentItem != null)
                {
                    if (shipmentItem.Amount < pickedItem.Value)
                    {
                        invalidItems.Add(pickedItem.Key);
                    }
                }
            }

            if (invalidItems.Any())
            {
                throw new InvalidOperationException($"Cannot pick the following items due to insufficient quantity: {string.Join(", ", invalidItems)}");
            }

            foreach (var pickedItem in pickedItems)
            {
                var shipmentItem = shipment.Items.FirstOrDefault(i => i.Item_Id == pickedItem.Key);
                if (shipmentItem != null)
                {
                    shipmentItem.Amount -= pickedItem.Value;
                    shipmentItem.CrossDockingStatus = shipmentItem.Amount == 0 ? "Picked" : "Partially Picked";
                }
            }

            shipment.Shipment_Status = shipment.Items.All(i => i.Amount == 0) ? "Picked" : "Partially Picked";
            shipment.Updated_At = DateTime.Now;

            SaveToFile(shipments);

            // Log the picking action
            var logEntry = new PickingLogEntry
            {
                Timestamp = DateTime.Now,
                PerformedBy = performedBy,
                ShipmentId = shipmentId,
                PickedItems = pickedItems,
                Description = description
            };

            await LogPickingAction(logEntry);
        }

        private async Task LogPickingAction(PickingLogEntry logEntry)
        {
            var logLine = $"Timestamp={logEntry.Timestamp:O} | PerformedBy={logEntry.PerformedBy} | ShipmentId={logEntry.ShipmentId} | Description={logEntry.Description} | PickedItems={string.Join(", ", logEntry.PickedItems.Select(kv => $"{kv.Key}:{kv.Value}"))}";

            await File.AppendAllTextAsync(logFilePath, logLine + Environment.NewLine);
        }
        
        public List<ItemDetail> GeneratePicklist(int shipmentId)
        {
            var shipment = GetById(shipmentId);
            if (shipment == null)
            {
                throw new KeyNotFoundException($"Shipment with ID {shipmentId} not found.");
            }

            return shipment.Items.Where(i => i.Amount > 0).ToList();
        }

    public Task Delete(int id)
    {
        var shipments = GetAll();
        var shipment = shipments.FirstOrDefault(s => s.Id == id);

        if (shipment == null)
        {
            throw new KeyNotFoundException($"Shipment with ID {id} not found.");
        }

        shipments.Remove(shipment);
        SaveToFile(shipments);

        return Task.CompletedTask;
    }

    public List<Shipment> GetAll(int? pageNumber = null, int? pageSize = null)
    {
        var jsonData = File.ReadAllText(jsonFilePath);
        var shipments = JsonConvert.DeserializeObject<List<Shipment>>(jsonData) ?? new List<Shipment>();

        if (pageNumber.HasValue && pageSize.HasValue && pageNumber > 0 && pageSize > 0)
        {
            shipments = shipments
                .Skip((pageNumber.Value - 1) * pageSize.Value)
                .Take(pageSize.Value)
                .ToList();
        }

        return shipments;
    }

    public Shipment GetById(int id)
    {
        var shipments = GetAll();
        return shipments.FirstOrDefault(s => s.Id == id) ?? throw new KeyNotFoundException($"Shipment with ID {id} not found.");
    }

    public Task Update(Shipment entity)
    {
        var shipments = GetAll();
        var existingShipment = shipments.FirstOrDefault(s => s.Id == entity.Id);

        if (existingShipment == null)
        {
            throw new KeyNotFoundException($"Shipment with ID {entity.Id} not found.");
        }

        // Ensure the existing shipment is replaced correctly
        shipments.Remove(existingShipment);
        shipments.Add(entity);

        Console.WriteLine($"Updating Shipment ID: {entity.Id}");
        Console.WriteLine(JsonConvert.SerializeObject(entity, Formatting.Indented));

        SaveToFile(shipments);

        return Task.CompletedTask;
    }


    private void SaveToFile(List<Shipment> shipments)
    {
        // Sort shipments by ID before saving
        var sortedShipments = shipments.OrderBy(s => s.Id).ToList();
        var jsonData = JsonConvert.SerializeObject(sortedShipments, Formatting.Indented);
        File.WriteAllText(jsonFilePath, jsonData);
    }

}
