namespace Ecommerce.Models; 

public class MenuCategoriasViewModel
{
    
    public List<Categoria> CategoriasVisiveis { get; set; } = new List<Categoria>();
   
    public List<Categoria> CategoriasDropdown { get; set; } = new List<Categoria>();    
    
}