using System.ComponentModel.DataAnnotations;

namespace HeladeriaAPI.Models.Ingrediente.Dto
{
    public class UpdateIngredienteDTO //DTO para actualizar un ingrediente.
    {
        [StringLength(100, ErrorMessage = "El nombre del ingrediente no puede exceder los 100 caracteres.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "El nombre del ingrediente solo puede contener letras y espacios.")]
        public string? Nombre { get; set; }
    }
}
