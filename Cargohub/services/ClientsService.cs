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
    public class ClientsService : ICrudService<Client, int>
    {
        private readonly string jsonFilePath = "data/clients.json";

        public Task Create(Client entity)
        {
            var clients = GetAll() ?? new List<Client>();

            // Find the next available ID
            var nextId = clients.Any() ? clients.Max(c => c.Id) + 1 : 1;
            entity.Id = nextId;

            clients.Add(entity);
            SaveToFile(clients);
            return Task.CompletedTask;
        }

        public Task Delete(int id)
        {
            var clients = GetAll() ?? new List<Client>();
            var client = clients.FirstOrDefault(c => c.Id == id);

            if (client == null)
            {
                throw new KeyNotFoundException($"Client with ID {id} not found.");
            }

            clients.Remove(client);
            SaveToFile(clients);
            return Task.CompletedTask;
        }

        public List<Client> GetAll()
        {
            var jsonData = File.ReadAllText(jsonFilePath);
            return JsonConvert.DeserializeObject<List<Client>>(jsonData) ?? new List<Client>();
        }

        public Client GetById(int id)
        {
            var clients = GetAll();
            var client = clients.FirstOrDefault(c => c.Id == id);

            if (client == null)
            {
                throw new KeyNotFoundException($"Client with ID {id} not found.");
            }

            return client;
        }

        public Task Update(int id, Client entity)
        {
            var clients = GetAll() ?? new List<Client>();
            var client = clients.FirstOrDefault(c => c.Id == id);

            if (client == null)
            {
                throw new KeyNotFoundException($"Client with ID {id} not found.");
            }

            client.Name = entity.Name;
            client.Email = entity.Email;
            client.Phone = entity.Phone;
            client.Address = entity.Address;

            SaveToFile(clients);
            return Task.CompletedTask;
        }

        private void SaveToFile(List<Client> clients)
        {
            var jsonData = JsonConvert.SerializeObject(clients, Formatting.Indented);
            File.WriteAllText(jsonFilePath, jsonData);
        }
    }
}