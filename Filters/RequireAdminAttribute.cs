using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Ecommerce.Filters
{
    
    public class RequireAdminAttribute : ActionFilterAttribute
    {        
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            
            string papel = context.HttpContext.Session.GetString("Papel");
            
            if (string.IsNullOrEmpty(papel) || papel != "Admin")
            {              
                
                context.Result = new RedirectToRouteResult(
                    new RouteValueDictionary
                    {
                        {"controller", "Auth"}, 
                        {"action", "Login"}
                    }
                );
            }
            
            base.OnActionExecuting(context);
        }
    }
}