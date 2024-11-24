using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cargohub.models;
using Newtonsoft.Json;

namespace Cargohub.services
{
    public static class AuthProvider
    {
        private static List<User> _users;
        private static readonly string filePath = Path.Combine("Data", "users.json");

        static AuthProvider()
        {
            LoadUsers();
        }

        private static void LoadUsers()
        {
            if (!Directory.Exists("Data"))
            {
                Directory.CreateDirectory("Data");
            }

            if (File.Exists(filePath))
            {
                var jsonData = File.ReadAllText(filePath);
                _users = JsonConvert.DeserializeObject<List<User>>(jsonData) ?? new List<User>();
            }
            else
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
                    }
                };
                SaveUsers();
            }
        }

         public static void ReloadUsers()
        {
            LoadUsers();
        }

        private static void SaveUsers()
        {
            var jsonData = JsonConvert.SerializeObject(_users, Formatting.Indented);
            File.WriteAllText(filePath, jsonData);
        }

        public static List<User> GetUsers()
        {
            return _users;
        }

        public static User GetUser(string apiKey)
        {
            return _users.FirstOrDefault(x => x.ApiKey == apiKey);
        }

        public static void AddUser(User user)
        {
            if (_users.Any(x => x.ApiKey == user.ApiKey))
            {
                throw new InvalidOperationException("A user with this API key already exists.");
            }
            _users.Add(user);
            SaveUsers();
        }

        public static void UpdateUser(string apiKey, User updatedUser)
        {
            var user = GetUser(apiKey);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            if (apiKey != updatedUser.ApiKey && _users.Any(x => x.ApiKey == updatedUser.ApiKey))
            {
                throw new InvalidOperationException("A user with this API key already exists.");
            }

            user.App = updatedUser.App;
            user.EndpointAccess = updatedUser.EndpointAccess;
            SaveUsers();
        }

        public static void DeleteUser(string apiKey)
        {
            var user = GetUser(apiKey);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }
            _users.Remove(user);
            SaveUsers();
        }

        public static bool HasAccess(User user, string path, string method)
        {
            // Check for full access
            if (user.EndpointAccess.ContainsKey("full") && user.EndpointAccess["full"].Full)
            {
                return true;
            }

            // Check for method-specific access
            if (user.EndpointAccess.ContainsKey("full"))
            {
                var access = user.EndpointAccess["full"];
                return method switch
                {
                    "get" => access.Get,
                    "post" => access.Post,
                    "put" => access.Put,
                    "delete" => access.Delete,
                    _ => false
                };
            }

            // Check for path-specific access
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