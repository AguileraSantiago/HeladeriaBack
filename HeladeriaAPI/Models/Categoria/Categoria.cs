using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HeladeriaAPI.Models.Helado;

namespace HeladeriaAPI.Models.Categoria
{
    public class Categoria
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string nombreCategoria { get; set; } = null!;

        public ICollection<Helado.Helado> Helados { get; set; } = new List<Helado.Helado>();
    }
}
