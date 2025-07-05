namespace HeladeriaAPI.Models.Helado.Dto
{
    public class AllHeladoDTO //DTO para mostrar un helado con todos sus datos relacionados
    {
        public int Id { get; set; }
        public string nombreHelado { get; set; } = null!;
        public double Precio { get; set; }
        public string nombreCategoria { get; set; } = null!;
        public string nombreEstado { get; set; } = null!;
        public List<string> Ingredientes { get; set; } = new List<string>();
    }
}
