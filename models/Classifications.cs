using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Cargohub.models
{
    public class Classifications
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [JsonProperty("created_at")]
        public DateTime Created_At { get; set; }

        [JsonProperty("updated_at")]
        public DateTime Updated_At { get; set; }
    }
}