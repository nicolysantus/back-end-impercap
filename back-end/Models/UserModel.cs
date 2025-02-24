using System.ComponentModel.DataAnnotations;

namespace back_end.Models
{
    public class UserModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "Nome de usuário é obrigatório.")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Tipo de usuário é obrigatório.")]
        public bool UserType { get; set; }

        [Required(ErrorMessage = "Nome é obrigatório.")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Sobrenome é obrigatório.")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "CPF é obrigatório.")]
        public string? CPF { get; set; }

        [Required(ErrorMessage = "Data de nascimento é obrigatória.")]
        public string? DateOfBirth { get; set; }

        [Required(ErrorMessage = "Email é obrigatório.")]
        [EmailAddress(ErrorMessage = "Email fornecido não é válido.")]
        public string? Email { get; set; }

        public string? Address { get; set; }

        [Required(ErrorMessage = "Telefone é obrigatório.")]
        public string? Number { get; set; }

        public string? Neighborhood { get; set; }
        public string? City { get; set; }

        [Required(ErrorMessage = "Senha é obrigatória.")]
        public string? Password { get; set; }
    }
}