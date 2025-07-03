//Estas dos líneas importan atributos necesarios para definir propiedades de la clase que se comportan como campos en una base de datos:
using System.ComponentModel.DataAnnotations; //permite usar atributos como [Key], [Required], etc.
using System.ComponentModel.DataAnnotations.Schema; //permite usar [DatabaseGenerated], para definir si un campo se genera automáticamente, etc.

namespace HeladeriaAPI.Models.Estado //indica que la clase Estado pertenece al submódulo Estado dentro de Models.
{
    public class Estado
    {
        [Key]//esta propiedad es la clave primaria de la tabla.
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]//la base de datos genera automáticamente un valor secuencial (auto-incremental) al crear una nueva fila.
        public int Id { get; set; } //identificador único del estado.
        public string Nombre { get; set; } = null!; //Nombre: representa el nombre del estado (por ejemplo, "Disponible", "Pendiente", etc.).
    }
}
