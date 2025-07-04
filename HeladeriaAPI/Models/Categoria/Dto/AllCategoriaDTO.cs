using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HeladeriaAPI.Models.Helado.Dto;

namespace HeladeriaAPI.Models.Categoria.Dto
{
    public class AllCategoriaDTO
    {
        public int Id { get; set; }
        public string nombreCategoria { get; set; } = null!;
        public ICollection<NombreHeladoDTO> Helados { get; set; } = new List<NombreHeladoDTO>();
    }
}
