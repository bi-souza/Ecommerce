
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;


namespace Ecommerce
{
    public class RequireLoginAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var http = context.HttpContext;
            var clienteId = http.Session.GetInt32("ClienteId");
            var adminId = http.Session.GetInt32("AdminId");

            if (clienteId is null && adminId is null)
            {
                context.Result = new RedirectResult("/Auth/Login");
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
