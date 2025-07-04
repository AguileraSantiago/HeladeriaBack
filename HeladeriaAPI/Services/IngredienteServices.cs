using AutoMapper; //para mapear DTOs a entidades.
using HeladeriaAPI.Config; //acceso a la base de datos.
using HeladeriaAPI.Models.Estado;
using HeladeriaAPI.Models.Helado;
using HeladeriaAPI.Models.Helado.Dto;
using HeladeriaAPI.Models.Ingrediente;
using HeladeriaAPI.Models.Ingrediente.Dto;
using HeladeriaAPI.Utils; //HttpError y HttpStatusCode: manejo de errores.
using Microsoft.EntityFrameworkCore; //EF Core para consultas a la base de datos.
using System.Net;

namespace HeladeriaAPI.Services
{
    public class IngredienteServices
    {
        //Inyecta dos dependencias:
        //IMapper: necesario para convertir entre DTOs y entidades.
        //ApplicationDbContext: necesario para interactuar con la DB.
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _db;
        public IngredienteServices(IMapper mapper, ApplicationDbContext db)
        {
            _mapper = mapper;
            _db = db;
        }

        //Método privado para evitar repetir código.
        private async Task<Ingrediente> GetOneByIdOrException(int id)
        {
            var ingrediente = await _db.Ingredientes.Where(h => h.Id == id).FirstOrDefaultAsync();

            if (ingrediente == null)//Si el ingrediente no existe, lanza un HttpError 404.
            {
                throw new HttpError($"No se encontro el Ingrediente con ID = {id}", HttpStatusCode.NotFound);
            }

            return ingrediente;
        }

        //Devuelve todos los ingredientes de la base de datos.
        public async Task<List<Ingrediente>> GetAll()
        {
            var ingredientes = await _db.Ingredientes.ToListAsync();
            return ingredientes;
        }

        //Devuelve todos los ingredientes cuyos Id estén en la lista.
        public async Task<List<Ingrediente>> GetAllByIds(List<int> ingredienteIds)
        {
            if (ingredienteIds == null || !ingredienteIds.Any()) //Valida que la lista no esté vacía ni sea nula. Si lo es, lanza error 400 (BadRequest).
            {
                throw new HttpError("La lista de IDs de Ingredientes no puede estar vacía.", HttpStatusCode.BadRequest);
            }
            var ingredientes = await _db.Ingredientes.Where(i => ingredienteIds.Contains(i.Id)).ToListAsync();
            return ingredientes;
        }

        // Usa el método privado para obtener un ingrediente y lanzar error si no existe.
        public async Task<Ingrediente> GetOneById(int id)
        {
            return await GetOneByIdOrException(id);
        }

        // Busca un ingrediente por nombre.
        public async Task<Ingrediente> GetOneByName(string nombre)
        {
            var ingrediente = await _db.Ingredientes.FirstOrDefaultAsync(e => e.nombreIngrediente == nombre);
            if (ingrediente == null)//Si no lo encuentra, lanza error 404 con mensaje claro.
            {
                throw new HttpError($"No se encontro el Ingrediente con Nombre = {nombre}", HttpStatusCode.NotFound);
            }
            return ingrediente;
        }

        //Usa AutoMapper para convertir el CreateIngredienteDTO en una entidad Ingrediente. Luego la guarda en la DB.
        public async Task<Ingrediente> CreateOne(CreateIngredienteDTO ingrediente)
        {

            var ing = _mapper.Map<Ingrediente>(ingrediente);

            await _db.Ingredientes.AddAsync(ing);
            await _db.SaveChangesAsync();
            return ing;
        }

        //Verifica que el ingrediente exista.
        //Luego usa AutoMapper.Map(source, destination) para actualizar las propiedades del objeto existente (ingredienteToUpdate) con las del DTO.
        //Guarda los cambios.
        public async Task<Ingrediente> UpdateOneById(int id, UpdateIngredienteDTO ingrediente)
        {
            var ingredienteToUpdate = await GetOneByIdOrException(id);
            

            var ingredienteUpdated = _mapper.Map(ingrediente, ingredienteToUpdate);

            _db.Ingredientes.Update(ingredienteUpdated);
            await _db.SaveChangesAsync();

            return ingredienteUpdated;
        }

        // Elimina un ingrediente si existe.
        //Después de borrar, verifica que realmente fue eliminado con AnyAsync.
        public async Task DeleteOneById(int id)
        {
            var ingrediente = await GetOneByIdOrException(id);
            _db.Ingredientes.Remove(ingrediente);
            await _db.SaveChangesAsync();
            if (await _db.Ingredientes.AnyAsync(hel => hel.Id == id)) // Si sigue existiendo, lanza error 500.
            {
                throw new HttpError($"No se pudo eliminar el Ingrediente con ID = {id}", HttpStatusCode.InternalServerError);
            }
        }
    }
}
