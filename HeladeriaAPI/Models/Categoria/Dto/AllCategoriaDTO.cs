using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HeladeriaAPI.Models.Categoria.Dto
{
    public class AllCategoriaDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
    }
}
