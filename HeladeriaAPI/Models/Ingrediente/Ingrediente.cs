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
    }

    public class IngredienteHelado //Esta clase representa una tabla intermedia (también llamada de unión) entre Helado e Ingrediente.
    {
        //Estas dos propiedades representan las claves foráneas hacia Helado e Ingrediente, respectivamente.
        public int IngredienteId { get; set; }
        public int HeladoId { get; set; }
    }
}
