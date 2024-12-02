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
      entity.Created_At = DateTime.UtcNow;
      entity.Updated_At = DateTime.UtcNow;

      transfers.Add(entity);
      SaveToFile(transfers);
      return Task.CompletedTask;
    }

    public List<ItemDetail> GetTransferItems(int transferId)
      {
          var transfer = GetById(transferId);
          return transfer?.Items ?? new List<ItemDetail>();
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

    public List<Transfer> GetAll(int? pageNumber = null, int? pageSize = null)
    {
        var jsonData = File.ReadAllText(jsonFilePath);
        var transfers = JsonConvert.DeserializeObject<List<Transfer>>(jsonData) ?? new List<Transfer>();

        // Apply pagination only if pageNumber and pageSize are provided and valid
        if (pageNumber.HasValue && pageSize.HasValue && pageNumber > 0 && pageSize > 0)
        {
            transfers = transfers
                .Skip((pageNumber.Value - 1) * pageSize.Value)
                .Take(pageSize.Value)
                .ToList();
        }

        return transfers;
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
      existingTransfer.Transfer_From = entity.Transfer_From;
      existingTransfer.Transfer_To = entity.Transfer_To;
      existingTransfer.Transfer_Status = entity.Transfer_Status;
      existingTransfer.Items = entity.Items;
      existingTransfer.Updated_At = DateTime.UtcNow;

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
