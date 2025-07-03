using System.ComponentModel.DataAnnotations;

namespace HeladeriaAPI.Models.Helado.Dto
{
    public class UpdateHeladoDTO //DTO para actualizar un helado existente.
    {
        [StringLength(100, ErrorMessage = "El nombre del helado no puede exceder los 100 caracteres.")]
        public string? Nombre { get; set; }

        [StringLength(255, ErrorMessage = "La descripción del helado no puede exceder los 255 caracteres.")]
        public string? Descripcion { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "El precio del helado debe ser mayor que cero.")]
        public double? Precio { get; set; }

        public bool? IsArtesanal { get; set; }

        [MinLength(1, ErrorMessage = "Debe haber al menos un ingrediente.")]
        public List<int>? IngredientesIds { get; set; }
    }
}
