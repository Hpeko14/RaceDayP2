using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RaceDayAPI.Helpers
{
    public class RoleAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string[] _roles;

        // Accept one or more roles. Example: [RoleAuthorize("organiser")] or [RoleAuthorize("organiser","participant")]
        public RoleAuthorizeAttribute(params string[] roles)
        {
            _roles = roles?.Where(r => !string.IsNullOrWhiteSpace(r))
                           .Select(r => r.Trim().ToLowerInvariant())
                           .ToArray() ?? Array.Empty<string>();
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var role = context.HttpContext.Session.GetString("Role");

            // Not authenticated (no session role)
            if (string.IsNullOrWhiteSpace(role))
            {
                context.Result = new UnauthorizedResult(); // 401
                return;
            }

            // If no roles specified on the attribute, allow any authenticated user
            if (_roles.Length == 0)
                return;

            // Check role case-insensitively
            var normalized = role.Trim().ToLowerInvariant();
            if (!_roles.Contains(normalized))
            {
                context.Result = new ForbidResult(); // 403
            }
        }
    }

}

