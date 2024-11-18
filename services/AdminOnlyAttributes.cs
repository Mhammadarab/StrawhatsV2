using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Collections.Generic;

public class AdminOnly : Attribute, IAsyncActionFilter
{
    private readonly List<string> _apiKeys = new List<string> { "a1b2c3d4e5", "f6g7h8i9j0", "k1l2m3n4o5" }; // Your list of API keys

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var headers = context.HttpContext.Request.Headers;

        if (!headers.ContainsKey("API_KEY"))
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        var authHeader = headers["API_KEY"].ToString();
        if (!_apiKeys.Contains(authHeader))
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        await next();
    }
}