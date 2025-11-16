using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Models;
    
public class Pessoa
{
    
    public int IdPessoa { get; set; } 

    [Required(ErrorMessage = "O nome é obrigatório.")]
    public string? Nome { get; set; }

    [Required(ErrorMessage = "O CPF é obrigatório.")]    
    [RegularExpression(@"^\d{11}$", ErrorMessage = "O CPF deve conter 11 números.")]
    public string? Cpf { get; set; }

    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "O telefone é obrigatório.")]
    [StringLength(11, MinimumLength = 11, ErrorMessage = "O telefone deve ter 11 dígitos.")]
    [RegularExpression(@"^\d{11}$", ErrorMessage = "O telefone deve conter 11 números")]
    public string? Telefone { get; set; }

    [Required(ErrorMessage = "Informe a data de nascimento.")]
    [DataType(DataType.Date)]    
    public DateTime DataNasc { get; set; }

    [Required(ErrorMessage = "A senha é obrigatória.")]
    [MinLength(7, ErrorMessage = "A senha deve ter no mínimo 7 caracteres.")]
    [DataType(DataType.Password)]
    public string? SenhaHash { get; set; }

    [DataType(DataType.Password)]
    [Compare("SenhaHash", ErrorMessage = "As senhas não conferem.")]
    public string? ConfirmarSenha { get; set; }
}
