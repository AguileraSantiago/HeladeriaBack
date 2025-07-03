using AutoMapper;
using HeladeriaAPI.Config;
using HeladeriaAPI.Models.Estado;
using HeladeriaAPI.Models.Helado;
using HeladeriaAPI.Models.Helado.Dto;
using HeladeriaAPI.Models.Ingrediente;
using HeladeriaAPI.Utils;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace HeladeriaAPI.Services
{
    public class HeladoServices
    {
        //Inyección de dependencias
        //IMapper: para mapear DTOs ↔ entidades
        //ApplicationDbContext: acceso a la DB.
        //IngredienteServices y EstadoServices: colaboración con servicios externos.
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _db;
        private readonly IngredienteServices _ingredienteServices;
        private readonly EstadoServices _estadoServices;
        public HeladoServices(IMapper mapper, ApplicationDbContext db, IngredienteServices ingredienteServices, EstadoServices estadoServices)
        {
            _mapper = mapper;
            _db = db;
            _ingredienteServices = ingredienteServices;
            _estadoServices = estadoServices;
        }

        // Busca un helado por ID incluyendo sus relaciones (Estado e Ingredientes).
        private async Task<Helado> GetOneByIdOrException(int id)
        {
            var helado = await _db.Helados
                .Where(h => h.Id == id)
                .Include(h => h.Estado)
                .Include(h => h.Ingredientes)
                .FirstOrDefaultAsync();

            if (helado == null) // Si no lo encuentra, lanza HttpError con código 404.
            {
                throw new HttpError($"No se encontro el helado con ID = {id}", HttpStatusCode.NotFound);
            }

            return helado;
        }

        //Devuelve una lista de todos los helados con su estado.
        // Mapea a DTOs (AllHeladoDTO) para respuesta liviana y estructurada.
        public async Task<List<AllHeladoDTO>> GetAll()
        {
            var heladosDb = await _db.Helados.Include(hel => hel.Estado).ToListAsync();
            var helados = _mapper.Map<List<AllHeladoDTO>>(heladosDb);
            return helados;
        }

        //Expone públicamente la lógica interna de búsqueda por ID.
        public async Task<Helado> GetOneById(int id)
        {
            return await GetOneByIdOrException(id);
        }

        //Crea un helado nuevo a partir del DTO.
        //Detalles importantes:
        //Asigna automáticamente el estado "Pendiente" usando EstadoServices.
        //Agrega el ingrediente "default" como mínimo inicial.
        // Esto asegura que ningún helado quede sin estado ni ingredientes.
        public async Task<Helado> CreateOne(CreateHeladoDTO helado) {

            var h = _mapper.Map<Helado>(helado);
            var estado = await _estadoServices.GetOneByName("Pendiente");
            h.Estado = estado;

            var ingredienteDefault = await _ingredienteServices.GetOneByName("default");
            h.Ingredientes = new List<Ingrediente> { ingredienteDefault };

            await _db.Helados.AddAsync(h);
            await _db.SaveChangesAsync();
            return h;
        }

        //Actualiza un helado existente.
        //Puntos importantes:
        //Actualiza las propiedades usando AutoMapper.Map(source, destination).
        //Si se pasan IDs de ingredientes, reemplaza los actuales por los nuevos.
        //Retorna el objeto original (heladoToUpdate), aunque el objeto actualizado es heladoUpdated. Puede que sea una pequeña incoherencia semántica, pero no rompe el funcionamiento.
        public async Task<Helado> UpdateOneById(int id, UpdateHeladoDTO helado)
        {
            var heladoToUpdate = await GetOneByIdOrException(id);
           
           var heladoUpdated = _mapper.Map(helado, heladoToUpdate);

            if (helado.IngredientesIds != null && helado.IngredientesIds.Any())
            {
                var ingredientes = await _ingredienteServices.GetAllByIds(helado.IngredientesIds);
                heladoUpdated.Ingredientes = ingredientes;
            }

            _db.Helados.Update(heladoUpdated);
            await _db.SaveChangesAsync();

            return heladoToUpdate;
        }

        // Borra un helado por ID.
        public async Task DeleteOneById(int id)
        {
            var helado = await GetOneByIdOrException(id);
            _db.Helados.Remove(helado);
            await _db.SaveChangesAsync();
            if (await _db.Helados.AnyAsync(hel => hel.Id == id)) //Verifica al final que el helado realmente fue eliminado. Si aún existe, lanza error 500.
            {
                throw new HttpError($"No se pudo eliminar el helado con ID = {id}", HttpStatusCode.InternalServerError);
            }
        }
          
    }
}
