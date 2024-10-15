using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.IO;
using Cargohub.interfaces;
using Cargohub.models;
using Newtonsoft.Json;

namespace Cargohub.services
{
    public class WarehouseService : ICrudService<Warehouse, int>
    {
        private readonly string jsonFilePath = "data/warehouses.json";
        public Task Create(Warehouse entity)
        {
            throw new NotImplementedException();
        }

        public Task Delete(int id)
        {
            throw new NotImplementedException();
        }

        public List<Warehouse> GetAll()
        {
            var jsonData = File.ReadAllText(jsonFilePath);
            return JsonConvert.DeserializeObject<List<Warehouse>>(jsonData);
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