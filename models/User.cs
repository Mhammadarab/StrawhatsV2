using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cargohub.models
{
    public enum UserRole
    {
        Admin,
        Manager,
        User,
        Guest
    }

    public class User
    {
        public string ApiKey { get; set; }
        public string App { get; set; }
        public Dictionary<string, EndpointAccess> EndpointAccess { get; set; }
        public UserRole Role { get; set; }
    }

    public class EndpointAccess
    {
        public bool Full { get; set; }
        public bool Get { get; set; }
        public bool Post { get; set; }
        public bool Put { get; set; }
        public bool Delete { get; set; }
    }

    public bool HasAccess(User user, string endpoint, string method)
    {
        if (user.Role == UserRole.Admin)
        {
            return true; // Admins have full access
        }

        if (user.EndpointAccess.TryGetValue(endpoint, out var access))
        {
            if (access.Full)
            {
                return true; // Full access to the endpoint
            }

            return method switch
            {
                "GET" => access.Get,
                "POST" => access.Post,
                "PUT" => access.Put,
                "DELETE" => access.Delete,
                _ => false
            };
        }

    return false;
    }
}