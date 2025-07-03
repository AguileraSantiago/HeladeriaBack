using System.ComponentModel.DataAnnotations;

namespace HeladeriaAPI.Models.Helado.Dto
{
    public class CreateHeladoDTO //DTO para crear un helado
    {
        [Required(ErrorMessage = "El nombre del helado es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre del helado no puede exceder los 100 caracteres.")]
        public string Nombre { get; set; } = null!;

        [Required(ErrorMessage = "La descripción del helado es obligatoria.")]
        [StringLength(255, ErrorMessage = "La descripción del helado no puede exceder los 255 caracteres.")]
        public string Descripcion { get; set; } = null!;

        [Required(ErrorMessage = "El precio del helado es obligatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio del helado debe ser mayor que cero.")]
        public double Precio { get; set; }

        [Required(ErrorMessage = "Debe especificar si el helado es artesanal o no.")]
        public bool IsArtesanal { get; set; }
    }
}
