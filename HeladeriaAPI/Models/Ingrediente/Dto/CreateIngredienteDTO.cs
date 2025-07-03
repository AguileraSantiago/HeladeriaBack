using System.ComponentModel.DataAnnotations;

namespace HeladeriaAPI.Models.Ingrediente.Dto
{
    public class CreateIngredienteDTO //DTO para crear un nuevo ingrediente
    {
        [Required(ErrorMessage = "El nombre del ingrediente es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre del ingrediente no puede exceder los 100 caracteres.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "El nombre del ingrediente solo puede contener letras y espacios.")]
        public string Nombre { get; set; } = null!;
    }
}
