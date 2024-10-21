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
    }

    public class EndpointAccess
    {
        public bool Full { get; set; }
        public bool Get { get; set; }
        public bool Post { get; set; }
        public bool Put { get; set; }
        public bool Delete { get; set; }
    }
}