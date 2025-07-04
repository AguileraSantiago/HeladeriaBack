using System.ComponentModel.DataAnnotations;

namespace HeladeriaAPI.Models.Helado.Dto
{
    public class CreateHeladoDTO
    {
        [Required(ErrorMessage = "El nombre del helado es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre del helado no puede exceder los 100 caracteres.")]
        public string nombreHelado { get; set; } = null!;

        [Required(ErrorMessage = "La descripción del helado es obligatoria.")]
        [StringLength(255, ErrorMessage = "La descripción del helado no puede exceder los 255 caracteres.")]
        public string Descripcion { get; set; } = null!;

        [Required(ErrorMessage = "El precio del helado es obligatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio del helado debe ser mayor que cero.")]
        public double Precio { get; set; }

        [Required(ErrorMessage = "Debe especificar si el helado es artesanal o no.")]
        public bool IsArtesanal { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una categoría.")]
        public int CategoriaId { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un estado.")]
        public int EstadoId { get; set; }
    }
}
