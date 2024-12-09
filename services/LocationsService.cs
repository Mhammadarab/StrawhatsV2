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
    public class LocationsService : ICrudService<Location, int>
    {
        private readonly string jsonFilePath = "data/locations.json";

        public async Task Create(Location entity)
        {
            var locations = GetAll() ?? new List<Location>();

            // Find the next available ID
            var nextId = locations.Any() ? locations.Max(l => l.Id) + 1 : 1;
            entity.Id = nextId;
            
            locations.Add(entity);
            await SaveToFile(locations);

        }

        public async Task Delete(int id)
        {
            var locations = GetAll() ?? new List<Location>();
            var location = locations.FirstOrDefault(l => l.Id == id);

            if (location == null)
            {
                throw new KeyNotFoundException($"Location with ID {id} not found.");
            }

            locations.Remove(location);
            await SaveToFile(locations);

        }

        public List<Location> GetAll(int? pageNumber = null, int? pageSize = null)
        {
            var jsonData = File.ReadAllText(jsonFilePath);
            var locations = JsonConvert.DeserializeObject<List<Location>>(jsonData) ?? new List<Location>();

            // Apply pagination only if pageNumber and pageSize are provided and valid
            if (pageNumber.HasValue && pageSize.HasValue && pageNumber > 0 && pageSize > 0)
            {
                locations = locations
                    .Skip((pageNumber.Value - 1) * pageSize.Value)
                    .Take(pageSize.Value)
                    .ToList();
            }

            return locations;
        }


        public Location GetById(int id)
        {
            var locations = GetAll();
            var location = locations.FirstOrDefault(l => l.Id == id);

            if (location == null)
            {
                throw new KeyNotFoundException($"Location with ID {id} not found.");
            }

            return location;
        }

        public async Task Update(Location entity)
        {
            var locations = GetAll() ?? new List<Location>();
            var location = locations.FirstOrDefault(l => l.Id == entity.Id);

            if (location == null)
            {
                throw new KeyNotFoundException($"Location with ID {entity.Id} not found.");
            }

            location.Id = entity.Id;
            location.Warehouse_Id = entity.Warehouse_Id;
            location.Code = entity.Code;
            location.Name = entity.Name;
            location.Created_At = entity.Created_At;
            location.Updated_At = entity.Updated_At;


            await SaveToFile(locations);
        }

        private async Task SaveToFile(List<Location> locations)
        {
            var jsonData = JsonConvert.SerializeObject(locations, Formatting.Indented);
            await File.WriteAllTextAsync(jsonFilePath, jsonData);
        }
    }
}