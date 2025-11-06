using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Models
{
    public class Cliente
    {
        public int IdCliente { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        public string? NomeCliente { get; set; }

        [Required(ErrorMessage = "O CPF é obrigatório.")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "O CPF deve ter 11 dígitos.")]
        public string? Cpf { get; set; }

        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "O telefone é obrigatório.")]
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
}