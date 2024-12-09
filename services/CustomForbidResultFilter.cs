using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class CustomForbidResultFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        // No custom logic here
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Result is ForbidResult)
        {
            context.Result = new JsonResult(new
            {
                error = "You do not have permission to perform this action. Please check your API key or permissions."
            })
            {
                StatusCode = StatusCodes.Status403Forbidden
            };
        }
    }
}