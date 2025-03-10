using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class SessionAuthorizationAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var session = context.HttpContext.Session;
        var userId = session.GetString("UserId");

        if (string.IsNullOrEmpty(userId))
        {
            context.Result = new UnauthorizedObjectResult("Unauthorized: No active session.");
        }
    }
}
