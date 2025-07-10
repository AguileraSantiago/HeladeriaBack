namespace HeladeriaAPI.Models.Ingrediente.Dto
{
    public class AllIngredienteHeladoDTO
    {
        public int HeladoId { get; set; }
        public string nombreHelado { get; set; } = null!;
        public List<IngredienteDTO> Ingredientes { get; set; } = new();
    }

    public class IngredienteDTO
    {
        public int IngredienteId { get; set; }
        public string nombreIngrediente { get; set; } = null!;
    }
}
