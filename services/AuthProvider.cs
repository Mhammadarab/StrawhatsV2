using System.Collections.Generic;
using System.Linq;
using Cargohub.models;

namespace Cargohub.services
{
    public static class AuthProvider
    {
        private static List<User> _users;

        public static void Init()
        {
            _users = new List<User>
            {
                new User
                {
                    ApiKey = "a1b2c3d4e5",
                    App = "CargoHUB Dashboard 1",
                    EndpointAccess = new Dictionary<string, EndpointAccess>
                    {
                        { "full", new EndpointAccess { Full = true } }
                    }
                },
                new User
                {
                    ApiKey = "f6g7h8i9j0",
                    App = "CargoHUB Dashboard 2",
                    EndpointAccess = new Dictionary<string, EndpointAccess>
                    {
                        { "full", new EndpointAccess { Full = false } },
                        { "warehouses", new EndpointAccess { Full = false, Get = true, Post = false, Put = false, Delete = false } },
                        { "locations", new EndpointAccess { Full = false, Get = true, Post = false, Put = false, Delete = false } },
                        { "transfers", new EndpointAccess { Full = false, Get = true, Post = false, Put = false, Delete = false } },
                        { "items", new EndpointAccess { Full = false, Get = true, Post = false, Put = false, Delete = false } },
                        { "item_lines", new EndpointAccess { Full = false, Get = true, Post = false, Put = false, Delete = false } },
                        { "item_groups", new EndpointAccess { Full = false, Get = true, Post = false, Put = false, Delete = false } },
                        { "item_types", new EndpointAccess { Full = false, Get = true, Post = false, Put = false, Delete = false } },
                        { "suppliers", new EndpointAccess { Full = false, Get = true, Post = false, Put = false, Delete = false } },
                        { "orders", new EndpointAccess { Full = false, Get = true, Post = false, Put = false, Delete = false } },
                        { "clients", new EndpointAccess { Full = false, Get = true, Post = false, Put = false, Delete = false } },
                        { "shipments", new EndpointAccess { Full = false, Get = true, Post = false, Put = false, Delete = false } }
                    }
                },
                new User
                {
                    ApiKey = "k1l2m3n4o",
                    App = "CargoHUB Dashboard 3",
                    EndpointAccess = new Dictionary<string, EndpointAccess>
                    {
                        { "full", new EndpointAccess { Full = false } },
                        { "warehouses", new EndpointAccess { Full = false, Get = true, Post = false, Put = false, Delete = false } },
                        { "locations", new EndpointAccess { Full = false, Get = true, Post = false, Put = false, Delete = false } },
                        { "transfers", new EndpointAccess { Full = false, Get = true, Post = false, Put = false, Delete = false } },
                        { "items", new EndpointAccess { Full = false, Get = true, Post = false, Put = false, Delete = false } },
                        { "item_lines", new EndpointAccess { Full = false, Get = true, Post = false, Put = false, Delete = false } },
                        { "item_groups", new EndpointAccess { Full = false, Get = true, Post = false, Put = false, Delete = false } },
                        { "item_types", new EndpointAccess { Full = false, Get = true, Post = false, Put = false, Delete = false } },
                        { "suppliers", new EndpointAccess { Full = false, Get = true, Post = false, Put = false, Delete = false } },
                        { "orders", new EndpointAccess { Full = false, Get = true, Post = false, Put = false, Delete = false } },
                        { "clients", new EndpointAccess { Full = false, Get = true, Post = false, Put = false, Delete = false } },
                        { "shipments", new EndpointAccess { Full = false, Get = true, Post = false, Put = false, Delete = false } }
                    }
                }
            };
        }

        public static User GetUser(string apiKey)
        {
            return _users.FirstOrDefault(x => x.ApiKey == apiKey);
        }

        public static bool HasAccess(User user, string path, string method)
        {
            if (user.EndpointAccess.ContainsKey("full") && user.EndpointAccess["full"].Full)
            {
                return true;
            }

            if (user.EndpointAccess.ContainsKey(path))
            {
                var access = user.EndpointAccess[path];
                return method switch
                {
                    "get" => access.Get,
                    "post" => access.Post,
                    "put" => access.Put,
                    "delete" => access.Delete,
                    _ => false
                };
            }

            return false;
        }
    }
}