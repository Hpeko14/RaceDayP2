using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RaceDayMVC.Services;

namespace RaceDayMVC.Filters;

/// <summary>
/// Protects MVC pages using the session. Not logged in: redirect to Login.
/// Logged in with the wrong role: redirect to Access Denied. Leave Role empty to allow any logged-in user.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class RoleAuthorizeAttribute : Attribute, IAuthorizationFilter
{
    public string? Role { get; set; }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var session = context.HttpContext.Session;
        if (session.GetInt32(SessionKeys.UserId) is null)
        {
            context.Result = new RedirectToActionResult("Login", "Account", null);
            return;
        }

        if (Role is not null && session.GetString(SessionKeys.Role) != Role)
            context.Result = new RedirectToActionResult("AccessDenied", "Account", null);
    }
}