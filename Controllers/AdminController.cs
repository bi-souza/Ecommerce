using Microsoft.AspNetCore.Mvc;
using Ecommerce.Filters; 

namespace Ecommerce.Controllers;

    [RequireAdmin]
    public class AdminController : Controller
    {
        
        public ActionResult Dashboard()
        {            
            return View();
        }
        
        
    }