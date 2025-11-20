using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Models; 
public class EstoqueInputViewModel
{
    [Required(ErrorMessage = "O Nível Mínimo é obrigatório.")]
    [Range(1, int.MaxValue, ErrorMessage = "Deve ser maior que zero.")]
    public int NivelMinimo { get; set; } = 5; 

    [Required(ErrorMessage = "O período é obrigatório.")]
    [Range(0, 365, ErrorMessage = "O período deve estar entre 0 e 365 dias.")]
    public int DiasRecentes { get; set; } = 30;
}