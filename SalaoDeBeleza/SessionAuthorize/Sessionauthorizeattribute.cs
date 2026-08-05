using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SalaoDeBeleza.Filters
{
    /// <summary>
    /// Filtro de autorização baseado em sessão.
    /// Verifica se o usuário está logado e possui o perfil (role) necessário.
    /// </summary>
    public class SessionAuthorizeAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Roles permitidas separadas por vírgula. Ex: "Admin,Gerente"
        /// Se vazio, qualquer usuário logado tem acesso.
        /// </summary>
        public string RoleAnyOf { get; set; } = "";

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;
            int? userId = session.GetInt32("UserId");

            // Usuário não está logado → redireciona para Login
            if (userId == null)
            {
                context.Result = new RedirectToActionResult("Login", "Admin",
                    new { returnUrl = context.HttpContext.Request.Path });
                return;
            }

            // Verifica o perfil, se informado
            if (!string.IsNullOrEmpty(RoleAnyOf))
            {
                string? userRole = session.GetString("Role");
                var rolesPermitidas = RoleAnyOf.Split(',', StringSplitOptions.TrimEntries);

                if (userRole == null || !rolesPermitidas.Contains(userRole))
                {
                    context.Result = new RedirectToActionResult("AcessoNegado", "Admin", null);
                    return;
                }
            }

            base.OnActionExecuting(context);
        }
    }
}