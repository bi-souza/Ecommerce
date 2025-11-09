using Microsoft.AspNetCore.Mvc;
using Ecommerce.Repositories;
using Ecommerce.Filters;
using Ecommerce.Models;


namespace Ecommerce.Controllers
{
    [RequireAdmin] 
    public class CategoriaController : Controller
    {
        private readonly ICategoriaRepository _repository;

        public CategoriaController(ICategoriaRepository repository)
        {
            _repository = repository;
        }

        
        public ActionResult Index()
        {
           
            List<Categoria> categorias = _repository.ReadAll();
            return View(categorias);
        }

        
        public ActionResult Create()
        {
            return View(new Categoria());
        }

        
        [HttpPost]         
        public ActionResult Create(Categoria categoria)
        {          
            
            _repository.Create(categoria);
            return RedirectToAction("Index");
        }

        
        public ActionResult Update(int id)
        {
            var categoria = _repository.ReadById(id);
            if (categoria == null)
            {
                return NotFound(); 
            }
            return View(categoria);
        }

        
        [HttpPost]
        public ActionResult Update(Categoria categoria)
        {            
            _repository.Update(categoria);
            return RedirectToAction("Index");
        }

        
        [HttpGet] 
        public ActionResult Delete(int id)
        {
            var categoria = _repository.ReadById(id);
            if (categoria == null)
            {
                return NotFound(); 
            }
            return View(categoria);
        }

        
        [HttpPost, ActionName("Delete")]         
        public ActionResult DeleteConfirmed(int id)       
        {
            
            _repository.Delete(id);
            return RedirectToAction("Index");
        }
    }
}






        
