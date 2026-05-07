using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;

namespace ProjetoOlimpicos.Filters
{

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class AllowAnonymousSessionAttribute : Attribute { }


    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class SessionAuthorizeAttribute : ActionFilterAttribute
    {
        public string? RoleAnyOf { get; set; }

        public string LoginController { get; set; } = "AdminController1";
        public string LoginAction { get; set; } = "Login";


        public string AccessDeniedController { get; set; } = "AdminController1";
        public string AccessDeniedAction { get; set; } = "AcessoNegado";

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var http = context.HttpContext;


            var allowAnon = context.ActionDescriptor?.EndpointMetadata?.Any(m => m is AllowAnonymousSessionAttribute) == true;
            if (allowAnon)
            {
                base.OnActionExecuting(context);
                return;
            }


            var userId = http.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {

                if (IsAjaxRequest(http))
                {
                    context.Result = new StatusCodeResult(StatusCodes.Status401Unauthorized);
                    return;
                }


                var returnUrl = http.Request.Path + http.Request.QueryString;

                context.Result = new RedirectToActionResult(
                    LoginAction,
                    LoginController,
                    new { returnUrl }
                );
                return;
            }


            if (!string.IsNullOrWhiteSpace(RoleAnyOf))
            {
                var role = http.Session.GetString("Role") ?? "Leitor";
                var allowed = RoleAnyOf.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                var ok = allowed.Contains(role, StringComparer.OrdinalIgnoreCase);
                if (!ok)
                {

                    if (IsAjaxRequest(http))
                    {
                        context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
                        return;
                    }

                    context.Result = new RedirectToActionResult(
                        AccessDeniedAction,
                        AccessDeniedController,
                        null
                    );
                    return;
                }
            }

            base.OnActionExecuting(context);
        }

        private static bool IsAjaxRequest(HttpContext http)
        {

            return string.Equals(
                http.Request.Headers["X-Requested-With"],
                "XMLHttpRequest",
                StringComparison.OrdinalIgnoreCase
            );
        }
    }
}

