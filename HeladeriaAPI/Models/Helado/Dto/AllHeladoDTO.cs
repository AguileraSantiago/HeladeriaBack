using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HeladeriaAPI.Models.Helado.Dto
{
    public class AllHeladoDTO //DTO para mostrar un helado con todos sus datos relacionados
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public double Precio { get; set; }

        public Estado.Estado Estado { get; set; } = null!;
    }
}
