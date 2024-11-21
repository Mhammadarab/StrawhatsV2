using Cargohub.interfaces;
using Cargohub.models;
using Newtonsoft.Json;

public class ShipmentService : ICrudService<Shipment, int>
{
    private readonly string jsonFilePath = "data/shipments.json";

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

    public List<Shipment> GetAll()
    {
        var jsonData = File.ReadAllText(jsonFilePath);
        return JsonConvert.DeserializeObject<List<Shipment>>(jsonData) ?? new List<Shipment>();
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
