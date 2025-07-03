using System.ComponentModel.DataAnnotations;

namespace HeladeriaAPI.Models.Categoria.Dto
{
    public class CreateCategoriaDTO
    {
        [Required(ErrorMessage = "El nombre de la categoría es obligatorio.")]
        [StringLength(50, ErrorMessage = "El nombre no puede superar los 50 caracteres.")]
        public string Nombre { get; set; } = null!;
    }
}
