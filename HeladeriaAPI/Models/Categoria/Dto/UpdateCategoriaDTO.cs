using System.ComponentModel.DataAnnotations;

namespace HeladeriaAPI.Models.Categoria.Dto
{
    public class UpdateCategoriaDTO
    {
        [StringLength(50, ErrorMessage = "El nombre no puede superar los 50 caracteres.")]
        public string nombreCategoria { get; set; } = null!;
    }
}
