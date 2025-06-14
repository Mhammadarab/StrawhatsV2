using Cargohub.interfaces;
using Cargohub.models;
using Newtonsoft.Json;

namespace Cargohub.services
{
    public class ClassificationService : ICrudService<Classifications, int>
    {
        private readonly string jsonFilePath = "data/classifications.json";

        public async Task Create(Classifications entity)
        {
            var classifications = GetAll() ?? new List<Classifications>();

            entity.Id = classifications.Any() ? classifications.Max(w => w.Id) + 1 : 1;
            entity.Created_At = DateTime.Now;
            entity.Updated_At = DateTime.Now;

            classifications.Add(entity);
            await SaveToFile(jsonFilePath, classifications);
        }

        public async Task Delete(int Id)
        {
            var Classificationss = GetAll() ?? new List<Classifications>();
            var Classifications = Classificationss.FirstOrDefault(c => c.Id == Id) ?? throw new KeyNotFoundException($"Classifications with Id {Id} not found.");
            Classificationss.Remove(Classifications);
            await SaveToFile(jsonFilePath, Classificationss);
        }

        public List<Classifications> GetAll(int? pageNumber = null, int? pageSize = null)
        {
            if (!File.Exists(jsonFilePath))
            {
                return new List<Classifications>();
            }

            var jsonData = File.ReadAllText(jsonFilePath);
            var classifications = JsonConvert.DeserializeObject<List<Classifications>>(jsonData) ?? new List<Classifications>();

            // Apply pagination only if pageNumber and pageSize are provided and valid
            if (pageNumber.HasValue && pageSize.HasValue && pageNumber > 0 && pageSize > 0)
            {
                classifications = classifications
                    .Skip((pageNumber.Value - 1) * pageSize.Value)
                    .Take(pageSize.Value)
                    .ToList();
            }

            return classifications;
        }


        public Classifications GetById(int Id)
        {
            var classifications = GetAll();
            var classification = classifications.FirstOrDefault(c => c.Id == Id);

            if (classification == null)
            {
                throw new KeyNotFoundException($"Classifications with Id {Id} not found.");
            }

            return classification;
        }

        public async Task Update(Classifications entity)
        {
            var Classificationss = GetAll() ?? new List<Classifications>();
            var existingClassifications = Classificationss.FirstOrDefault(c => c.Id == entity.Id);

            if (existingClassifications == null)
            {
                throw new KeyNotFoundException($"Classifications with Id {entity.Id} not found.");
            }

            existingClassifications.Name = entity.Name;
            existingClassifications.Updated_At = DateTime.Now;

            await SaveToFile(jsonFilePath, Classificationss);
        }

        private async Task SaveToFile<T>(string filePath, List<T> data)
        {
            var jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);
            await File.WriteAllTextAsync(filePath, jsonData);
        }
    }
}