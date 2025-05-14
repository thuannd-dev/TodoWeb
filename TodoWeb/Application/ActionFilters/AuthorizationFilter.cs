using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TodoWeb.Constants.Enums;

namespace TodoWeb.Application.ActionFilters
{
    public class AuthorizationFilter : IAuthorizationFilter
    {
        private readonly string[] _userRole;

        public AuthorizationFilter(string userRole)
        {
            _userRole = userRole.Split(",");
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var userId = context.HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var role = context.HttpContext.Session.GetString("Role");
            if (!_userRole.Contains(role))
            {
                context.Result = new StatusCodeResult(403);
            }
        }
    }
}
