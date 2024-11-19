using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Cargohub.models;

public class AdminOnly : Attribute, IAsyncActionFilter
{
    private static List<User> _users;
    private static readonly string filePath = Path.Combine("Data", "users.json");

    static AdminOnly()
    {
        LoadUsers();
    }

    private static void LoadUsers()
    {
        if (File.Exists(filePath))
        {
            var jsonData = File.ReadAllText(filePath);
            _users = JsonConvert.DeserializeObject<List<User>>(jsonData) ?? new List<User>();
        }
        else
        {
            _users = new List<User>();
        }
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var headers = context.HttpContext.Request.Headers;

        if (!headers.ContainsKey("API_KEY"))
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        var apiKey = headers["API_KEY"].ToString();
        var user = _users.FirstOrDefault(u => u.ApiKey == apiKey);

        if (user == null)
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        await next();
    }
}