using Microsoft.AspNetCore.Mvc;
using Ecommerce.Repositories;
using Ecommerce.Models;

public class MenuCategoriasViewComponent : ViewComponent
{
    private readonly ICategoriaRepository _repository;

    public MenuCategoriasViewComponent(ICategoriaRepository repository)
    {
        _repository = repository;
    }

    public IViewComponentResult Invoke()
    {
        
        var todasCategorias = _repository.ReadAll();         
        
        var categoriasVM = new MenuCategoriasViewModel 
        {
            CategoriasVisiveis = todasCategorias.Take(4).ToList(),
            CategoriasDropdown = todasCategorias.Skip(4).ToList()
        };
        
        return View(categoriasVM);
    }


}