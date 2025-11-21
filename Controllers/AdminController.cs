using Microsoft.AspNetCore.Mvc;
using Ecommerce.Repositories;
using Ecommerce.Filters; 
using Ecommerce.Models;

namespace Ecommerce.Controllers;

[RequireAdmin]
    public class AdminController : Controller
    {
        private IProdutoRepository produtoRepository;

        public AdminController(IProdutoRepository produtoRepository)
        {
             this.produtoRepository = produtoRepository;
        }
        
        public ActionResult Dashboard()
        {            
            return View();
        }
        
        [HttpGet]
        [RequireAdmin] 
        public IActionResult EstoqueCritico()
        {
            return View(new EstoqueInputViewModel());
        }

        [HttpPost]
        [RequireAdmin]    
        public ActionResult EstoqueCritico(EstoqueInputViewModel inputModel)
        {        
            if (!ModelState.IsValid)
            {                
                return View(inputModel); 
            }            
            
            var relatorio = produtoRepository.ReadEstoqueCritico(
                inputModel.NivelMinimo, 
                inputModel.DiasRecentes
            );
            
            ViewBag.Relatorio = relatorio;             
            
            return View(inputModel);
        }        
        
    }