using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Models;

public class ClientePerfilViewModel
{
    
    public int IdCliente { get; set; } 

    [Required(ErrorMessage = "O nome é obrigatório.")]
    public string? Nome { get; set; }

    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "O telefone é obrigatório.")]
    [StringLength(11, MinimumLength = 11, ErrorMessage = "O telefone deve ter 11 dígitos.")]
    [RegularExpression(@"^\d{11}$", ErrorMessage = "O telefone deve conter 11 números")]
    public string? Telefone { get; set; }
    
}