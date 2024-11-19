using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cargohub.models;

namespace Cargohub.services
{
    public class UserService
    {
        private static List<User> _users = new List<User>();

        public List<User> GetAll()
        {
            return _users;
        }

        public User GetById(string apiKey)
        {
            return _users.FirstOrDefault(x => x.ApiKey == apiKey);
        }

        public async Task Create(User user)
        {
            if (_users.Any(x => x.ApiKey == user.ApiKey))
            {
                throw new InvalidOperationException("A user with this API key already exists.");
            }
            _users.Add(user);
            await Task.CompletedTask;
        }

        public async Task Update(string apiKey, User updatedUser)
        {
            var user = GetById(apiKey);
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
            await Task.CompletedTask;
        }

        public async Task Update(User updatedUser)
        {
            var user = GetById(updatedUser.ApiKey);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            if (_users.Any(x => x.ApiKey == updatedUser.ApiKey && x != user))
            {
                throw new InvalidOperationException("A user with this API key already exists.");
            }

            user.App = updatedUser.App;
            user.EndpointAccess = updatedUser.EndpointAccess;
            await Task.CompletedTask;
        }

        public async Task Delete(string apiKey)
        {
            var user = GetById(apiKey);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }
            _users.Remove(user);
            await Task.CompletedTask;
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