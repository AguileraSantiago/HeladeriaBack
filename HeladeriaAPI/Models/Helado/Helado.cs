using HeladeriaAPI.Models.Categoria;
using HeladeriaAPI.Models.Estado;
using HeladeriaAPI.Models.Ingrediente; //Importa el namespace donde está Ingrediente, porque se usará para la lista de ingredientes del helado.
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace HeladeriaAPI.Models.Helado
{
    public class Helado
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string nombreHelado { get; set; } = null!;
        public string Descripcion { get; set; } = null!;
        public double Precio { get; set; }
        public bool IsArtesanal { get; set; }

        [ForeignKey("EstadoId")] 
        public Estado.Estado Estado { get; set; } = null!; //Propiedad de navegación: EF Core cargará el objeto Estado completo (no solo el Id), si se lo pide.
        public int EstadoId { get; set; } //clave foránea que indica en qué estado se encuentra el helado

        public List<Ingrediente.Ingrediente> Ingredientes { get; set; } //Es una relación muchos a muchos configurada en OnModelCreating usando la clase intermedia IngredienteHelado.
        public DateTime FechaCreacion { get; set; } //En OnModelCreating, se configura para que se asigne automáticamente con GETUTCDATE() al insertar un nuevo registro.

        [Required]
        public int CategoriaId { get; set; }

        public Categoria.Categoria Categoria { get; set; } = null!;
    }
}
