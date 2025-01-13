using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cargohub.models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Cargohub.services
{
    public static class AuthProvider
    {
        private static List<User> _users;
        private static readonly string filePath = Path.Combine("Data", "users.json");
        private static readonly string logFilePath = Path.Combine("Logs", "user_changes.json");

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
                        ApiKey = "owner",
                        App = "CargoHUB Dashboard 1",
                        EndpointAccess = new Dictionary<string, EndpointAccess>
                        {
                            { "clients", new EndpointAccess { All = true, Single = true, Create = true, Update = true, Delete = true } },
                            { "orders", new EndpointAccess { All = true, Single = true, Create = true, Update = true, Delete = true } },
                            { "inventories", new EndpointAccess { All = true, Single = true, Create = true, Update = true, Delete = true } },
                            { "shipments", new EndpointAccess { All = true, Single = true, Create = true, Update = true, Delete = true } },
                            { "suppliers", new EndpointAccess { All = true, Single = true, Create = true, Update = true, Delete = true } },
                            { "items", new EndpointAccess { All = true, Single = true, Create = true, Update = true, Delete = true } },
                            { "warehouses", new EndpointAccess { All = true, Single = true, Create = true, Update = true, Delete = true } },
                            { "transfers", new EndpointAccess { All = true, Single = true, Create = true, Update = true, Delete = true } },
                            { "locations", new EndpointAccess { All = true, Single = true, Create = true, Update = true, Delete = true } },
                            { "item_types", new EndpointAccess { All = true, Single = true, Create = true, Update = true, Delete = true } },
                            { "item_lines", new EndpointAccess { All = true, Single = true, Create = true, Update = true, Delete = true } },
                            { "item_groups", new EndpointAccess { All = true, Single = true, Create = true, Update = true, Delete = true } }
                        },
                        Warehouses = new List<int> { 1, 2, 3, 4, 5 }
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
        public static void DeactivateUser(string performedBy, string apiKey)
        {
            var user = GetUser(apiKey);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            user.IsActive = false;
            SaveUsers();
            LogChange("Deactivated", performedBy, oldUser: user);
        }

        public static void ReactivateUser(string performedBy, string apiKey)
        {
            var user = GetUser(apiKey);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            user.IsActive = true;
            SaveUsers();
            LogChange("Reactivated", performedBy, oldUser: user);
        }

        private static void LogChange(string action, string performedBy, User oldUser = null, User newUser = null)
        {
            var changes = new Dictionary<string, object>();

            if (oldUser != null && newUser != null)
            {
                changes = GetUpdatedFields(oldUser, newUser);
            }
            else if (oldUser != null)
            {
                changes["DeletedUser"] = oldUser;
            }
            else if (newUser != null)
            {
                changes["NewUser"] = newUser;
            }

            if (changes.Count == 0)
                return; // Nothing to log if there are no changes.

            var APIkey = oldUser?.ApiKey ?? newUser?.ApiKey;
            var logEntry = new JObject
            {
                ["Timestamp"] = DateTime.UtcNow,
                ["Action"] = action,
                ["PerformedBy"] = performedBy,
                ["APIkey"] = APIkey,
                ["Changes"] = JObject.FromObject(changes)
            };

            Directory.CreateDirectory("logs");

            List<JObject> logs;
            if (File.Exists(logFilePath))
            {
                var jsonData = File.ReadAllText(logFilePath);
                logs = JsonConvert.DeserializeObject<List<JObject>>(jsonData) ?? new List<JObject>();
            }
            else
            {
                logs = new List<JObject>();
            }

            logs.Add(logEntry);
            File.WriteAllText(logFilePath, JsonConvert.SerializeObject(logs, Formatting.Indented));
        }

        private static Dictionary<string, object> GetUpdatedFields(User oldUser, User newUser)
        {
            var changes = new Dictionary<string, object>();

            if (oldUser.ApiKey != newUser.ApiKey)
                changes["ApiKey"] = new { OldValue = oldUser.ApiKey, NewValue = newUser.ApiKey };

            if (oldUser.App != newUser.App)
                changes["App"] = new { OldValue = oldUser.App, NewValue = newUser.App };

            var endpointAccessChanges = new Dictionary<string, object>();
            foreach (var key in newUser.EndpointAccess.Keys)
            {
                // Check if the old user has the key and compare the values
                if (!oldUser.EndpointAccess.TryGetValue(key, out var oldAccess))
                {
                    // If the key doesn't exist in the old user, add the entire access object
                    endpointAccessChanges[key] = new { OldValue = (EndpointAccess)null, NewValue = newUser.EndpointAccess[key] };
                }
                else
                {
                    // Check for individual property changes
                    var accessChanges = new Dictionary<string, object>();
                    if (oldAccess.All != newUser.EndpointAccess[key].All)
                        accessChanges["All"] = new { OldValue = oldAccess.All, NewValue = newUser.EndpointAccess[key].All };
                    if (oldAccess.Single != newUser.EndpointAccess[key].Single)
                        accessChanges["Single"] = new { OldValue = oldAccess.Single, NewValue = newUser.EndpointAccess[key].Single };
                    if (oldAccess.Create != newUser.EndpointAccess[key].Create)
                        accessChanges["Create"] = new { OldValue = oldAccess.Create, NewValue = newUser.EndpointAccess[key].Create };
                    if (oldAccess.Update != newUser.EndpointAccess[key].Update)
                        accessChanges["Update"] = new { OldValue = oldAccess.Update, NewValue = newUser.EndpointAccess[key].Update };
                    if (oldAccess.Delete != newUser.EndpointAccess[key].Delete)
                        accessChanges["Delete"] = new { OldValue = oldAccess.Delete, NewValue = newUser.EndpointAccess[key].Delete };

                    // Only add the key if there are changes
                    if (accessChanges.Count > 0)
                        endpointAccessChanges[key] = accessChanges;
                    
                    if (!oldUser.Warehouses.SequenceEqual(newUser.Warehouses))
                        changes["Warehouses"] = new { OldValue = oldUser.Warehouses, NewValue = newUser.Warehouses };
                }
            }

            if (endpointAccessChanges.Count > 0)
                changes["EndpointAccess"] = endpointAccessChanges;

            return changes;
        }

        public static List<User> GetUsers(int? pageNumber = null, int? pageSize = null)
        {
            // Ensure _users is not null
            var users = _users ?? new List<User>();

            // Apply pagination only if pageNumber and pageSize are provided and valid
            if (pageNumber.HasValue && pageSize.HasValue && pageNumber > 0 && pageSize > 0)
            {
                users = users
                    .Skip((pageNumber.Value - 1) * pageSize.Value)
                    .Take(pageSize.Value)
                    .ToList();
            }

            return users;
        }

        public static User GetUser(string apiKey)
        {
            var user = _users.FirstOrDefault(x => x.ApiKey == apiKey);
            return user;
        }

        public static void AddUser(string performedBy, User user)
        {
            if (_users.Any(x => x.ApiKey == user.ApiKey))
            {
                throw new InvalidOperationException("A user with this API key already exists.");
            }

            _users.Add(user);
            SaveUsers();
            LogChange("Created", performedBy, newUser: user);
        }

        public static void UpdateUser(string performedBy, string apiKey, User updatedUser)
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

            var oldUser = JsonConvert.DeserializeObject<User>(JsonConvert.SerializeObject(user)); // Deep clone
            user.App = updatedUser.App;
            user.EndpointAccess = updatedUser.EndpointAccess;
            user.Warehouses = updatedUser.Warehouses;

            SaveUsers();
            LogChange("Updated", performedBy, oldUser, updatedUser);
        }

        public static void DeleteUser(string performedBy, string apiKey)
        {
            var user = GetUser(apiKey);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            _users.Remove(user);
            SaveUsers();

            // Log the deletion of the user
            LogChange("Deleted", performedBy, oldUser: user);
        }

        public static bool HasAccess(User user, string path, string permission)
        {
            // Check for path-specific access
            if (user.EndpointAccess.TryGetValue(path, out var specificAccess))
            {
                return permission switch
                {
                    "single" => specificAccess.Single,
                    "all" => specificAccess.All,
                    "post" => specificAccess.Create,
                    "put" => specificAccess.Update,
                    "delete" => specificAccess.Delete, 
                    _ => false
                };
            }

            // Deny access by default
            return false;
        }

        public static bool HasWarehouseAccess(string apiKey, int warehouseId)
        {
            var user = GetUser(apiKey);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            return user.Warehouses.Contains(warehouseId);
        }
        public static void AddWarehouse(string performedBy, string apiKey, int warehouseId)
        {
            var user = GetUser(apiKey);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            if (!user.Warehouses.Contains(warehouseId))
            {
                var oldUser = JsonConvert.DeserializeObject<User>(JsonConvert.SerializeObject(user)); // Deep clone
                user.Warehouses.Add(warehouseId);
                UpdateUser(performedBy, apiKey, user);
                LogChange("AddedWarehouse", performedBy, oldUser, user);
            }
        }

        public static void RemoveWarehouse(string performedBy, string apiKey, int warehouseId)
        {
            var user = GetUser(apiKey);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            if (user.Warehouses.Contains(warehouseId))
            {
                var oldUser = JsonConvert.DeserializeObject<User>(JsonConvert.SerializeObject(user)); // Deep clone
                user.Warehouses.Remove(warehouseId);
                UpdateUser(performedBy, apiKey, user);
                LogChange("RemovedWarehouse", performedBy, oldUser, user);
            }
            else
            {
                throw new InvalidOperationException("Warehouse ID not found in the user's list.");
            }
        }
    }
}