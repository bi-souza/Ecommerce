using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Ecommerce.Filters
{
    
    public class RequireAdminAttribute : ActionFilterAttribute
    {        
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            
            string papel = context.HttpContext.Session.GetString("Papel");
            
            if (string.IsNullOrEmpty(papel) || papel != "Admin")
            {              
                
                context.Result = new RedirectResult("/Auth/Login");                    
                
            }
            
            base.OnActionExecuting(context);
        }
    }
}