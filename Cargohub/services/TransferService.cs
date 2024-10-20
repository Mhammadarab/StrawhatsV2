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
  public class TransferService : ICrudService<Transfer, int>
  {
    private readonly string jsonFilePath = "data/transfers.json";

    public Task Create(Transfer entity)
    {
      var transfers = GetAll() ?? new List<Transfer>();

      // Find the next available ID
      var nextId = transfers.Any() ? transfers.Max(t => t.Id) + 1 : 1;
      entity.Id = nextId;
      entity.CreatedAt = DateTime.UtcNow;
      entity.UpdatedAt = DateTime.UtcNow;

      transfers.Add(entity);
      SaveToFile(transfers);
      return Task.CompletedTask;
    }

    public Task Delete(int id)
    {
      var transfers = GetAll() ?? new List<Transfer>();
      var transfer = transfers.FirstOrDefault(t => t.Id == id);

      if (transfer == null)
      {
        throw new KeyNotFoundException($"Transfer with ID {id} not found.");
      }

      transfers.Remove(transfer);
      SaveToFile(transfers);
      return Task.CompletedTask;
    }

    public List<Transfer> GetAll()
    {
      var jsonData = File.ReadAllText(jsonFilePath);
      return JsonConvert.DeserializeObject<List<Transfer>>(jsonData) ?? new List<Transfer>();
    }

    public Transfer GetById(int id)
    {
      var transfers = GetAll();
      var transfer = transfers.FirstOrDefault(t => t.Id == id);

      if (transfer == null)
      {
        throw new KeyNotFoundException($"Transfer with ID {id} not found.");
      }

      return transfer;
    }

    public Task Update(Transfer entity)
    {
      var transfers = GetAll() ?? new List<Transfer>();
      var existingTransfer = transfers.FirstOrDefault(t => t.Id == entity.Id);

      if (existingTransfer == null)
      {
        throw new KeyNotFoundException($"Transfer with ID {entity.Id} not found.");
      }

      // Update properties
      existingTransfer.Reference = entity.Reference;
      existingTransfer.TransferFrom = entity.TransferFrom;
      existingTransfer.TransferTo = entity.TransferTo;
      existingTransfer.TransferStatus = entity.TransferStatus;
      existingTransfer.Items = entity.Items;
      existingTransfer.UpdatedAt = DateTime.UtcNow;

      SaveToFile(transfers);
      return Task.CompletedTask;
    }

    private void SaveToFile(List<Transfer> transfers)
    {
      var jsonData = JsonConvert.SerializeObject(transfers, Formatting.Indented);
      File.WriteAllText(jsonFilePath, jsonData);
    }
  }
}
