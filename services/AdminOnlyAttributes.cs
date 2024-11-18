using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

public class AdminOnly : Attribute, IAsyncActionFilter
{
    private readonly string _apiKey = "a1b2c3d4e5"; // Your API key

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var headers = context.HttpContext.Request.Headers;

        if (!headers.ContainsKey("API_KEY"))
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        var authHeader = headers["API_KEY"].ToString();
        if (authHeader != _apiKey)
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        await next();
    }
}