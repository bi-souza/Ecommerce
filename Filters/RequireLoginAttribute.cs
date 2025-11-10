
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;

namespace Ecommerce
{
    public class RequireLoginAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var http = context.HttpContext;
            var clienteId = http.Session.GetInt32("ClienteId");

            if (clienteId is null)
            {
                context.Result = new RedirectResult("/Auth/Login");
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
