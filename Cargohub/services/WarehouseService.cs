using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Cargohub.interfaces;
using Cargohub.models;

namespace Cargohub.services
{
    public class WarehouseService : ICrudService<Warehouse, int>
    {
        public Task Create(Warehouse entity)
        {
            throw new NotImplementedException();
        }

        public Task Delete(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Warehouse> GetAll()
        {
            var dataPath = "data/warehouses.json";
            if (!File.Exists(dataPath))
            {
                return new List<Warehouse>();
            }

            var jsonData = File.ReadAllText(dataPath);
            var warehouses = JsonSerializer.Deserialize<List<Warehouse>>(jsonData) ?? new List<Warehouse>();
            return warehouses;
        }
        public Warehouse GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Task Update(Warehouse entity)
        {
            throw new NotImplementedException();
        }
    }
}