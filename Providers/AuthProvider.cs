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
        private static readonly string logFilePath = Path.Combine("Logs", "user_logs.json");

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

        private static void LogChange(string action, string performedBy, User oldUser = null, User newUser = null)
        {
            if (oldUser == null || newUser == null)
                throw new ArgumentNullException("OldUser and NewUser cannot be null.");

            var changes = GetUpdatedFields(oldUser, newUser);

            if (changes.Count == 0)
                return; // Nothing to log if there are no changes.

            var logEntry = new
            {
                Timestamp = DateTime.UtcNow,
                Action = action,
                PerformedBy = performedBy,
                ApiKey = newUser.ApiKey, // Log the key being updated
                Changes = changes
            };

            var logFilePath = Path.Combine("logs", "user_changes.json");
            Directory.CreateDirectory("logs");

            List<object> logs;
            if (File.Exists(logFilePath))
            {
                var jsonData = File.ReadAllText(logFilePath);
                logs = JsonConvert.DeserializeObject<List<object>>(jsonData) ?? new List<object>();
            }
            else
            {
                logs = new List<object>();
            }

            logs.Add(logEntry);
            File.WriteAllText(logFilePath, JsonConvert.SerializeObject(logs, Formatting.Indented));
        }

        private static Dictionary<string, object> GetUpdatedFields(User oldUser, User newUser)
        {
            var changes = new Dictionary<string, object>();

            if (oldUser.ApiKey != newUser.ApiKey)
                changes["ApiKey"] = newUser.ApiKey;

            if (oldUser.App != newUser.App)
                changes["App"] = newUser.App;

            var endpointAccessChanges = new Dictionary<string, object>();

            foreach (var key in newUser.EndpointAccess.Keys)
            {
                // Check if the old user has the key and compare the values
                if (!oldUser.EndpointAccess.TryGetValue(key, out var oldAccess))
                {
                    // If the key doesn't exist in the old user, add the entire access object
                    endpointAccessChanges[key] = newUser.EndpointAccess[key];
                }
                else
                {
                    // Check for individual property changes
                    var accessChanges = new Dictionary<string, bool>();
                    if (oldAccess.Full != newUser.EndpointAccess[key].Full)
                        accessChanges["Full"] = newUser.EndpointAccess[key].Full;

                    if (oldAccess.Get != newUser.EndpointAccess[key].Get)
                        accessChanges["Get"] = newUser.EndpointAccess[key].Get;

                    if (oldAccess.Post != newUser.EndpointAccess[key].Post)
                        accessChanges["Post"] = newUser.EndpointAccess[key].Post;

                    if (oldAccess.Put != newUser.EndpointAccess[key].Put)
                        accessChanges["Put"] = newUser.EndpointAccess[key].Put;

                    if (oldAccess.Delete != newUser.EndpointAccess[key].Delete)
                        accessChanges["Delete"] = newUser.EndpointAccess[key].Delete;

                    // Only add the key if there are changes
                    if (accessChanges.Count > 0)
                        endpointAccessChanges[key] = accessChanges;
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
            return _users.FirstOrDefault(x => x.ApiKey == apiKey);
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


        public static bool HasAccess(User user, string path, string method)
        {
            // Check for full access
            if (user.EndpointAccess.TryGetValue("full", out var fullAccess) && fullAccess.Full)
            {
                return true;
            }

            // Check for path-specific access
            if (user.EndpointAccess.TryGetValue(path, out var specificAccess))
            {
                return method switch
                {
                    "get" => specificAccess.Get,
                    "post" => specificAccess.Post,
                    "put" => specificAccess.Put,
                    "delete" => specificAccess.Delete,
                    _ => false
                };
            }

            // Deny access by default
            return false;
        }
    }
}
