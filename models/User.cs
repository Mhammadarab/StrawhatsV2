using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cargohub.models
{

    public class User
    {
        public string ApiKey { get; set; }
        public string App { get; set; }
        public Dictionary<string, EndpointAccess> EndpointAccess { get; set; }
        public bool IsActive { get; set; } = true;
        public List<int> Warehouses { get; set; }
    }

    public class EndpointAccess
    {
        public bool All { get; set; } // GET all
        public bool Single { get; set; } // GET single
        public bool Create { get; set; } // POST
        public bool Update { get; set; } // PUT
        public bool Delete { get; set; } // DELETE
    }
}