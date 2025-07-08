using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HeladeriaAPI.Models.Ingrediente
{
    public class Ingrediente
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string nombreIngrediente { get; set; } = null!;

        // public ICollection<IngredienteHelado> IngredienteHelados { get; set; } = new List<IngredienteHelado>();
    }

    public class IngredienteHelado //Esta clase representa una tabla intermedia (también llamada de unión) entre Helado e Ingrediente.
    {
        //Estas dos propiedades representan las claves foráneas hacia Helado e Ingrediente, respectivamente.
        public int HeladoId { get; set; }
        public Helado.Helado Helado { get; set; } = null!;

        public int IngredienteId { get; set; }
        public Ingrediente Ingrediente { get; set; } = null!;

    }
}
