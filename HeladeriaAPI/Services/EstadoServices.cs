using System.Net;//para usar HttpStatusCode, como por ejemplo HttpStatusCode.NotFound.
using AutoMapper;
using HeladeriaAPI.Config;//se usa para acceder a la clase ApplicationDbContext, que maneja la base de datos.
using HeladeriaAPI.Models.Estado;
using HeladeriaAPI.Utils;//se usa para la clase personalizada HttpError.
using Microsoft.EntityFrameworkCore;//para utilizar métodos como .Where(), .ToListAsync() y .FirstOrDefaultAsync().

namespace HeladeriaAPI.Services
{
    public class EstadoServices
    {
        //dos dependencias inyectadas por el constructor
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _db;
        public EstadoServices(IMapper mapper, ApplicationDbContext db)
        {
            _mapper = mapper;
            _db = db;
        }

        //Método privado que busca un Estado por su ID.
        private async Task<Estado> GetOneByIdOrException(int id)
        {
            var estado = await _db.Estados.Where(h => h.Id == id).FirstOrDefaultAsync();

            if (estado == null) //Si no lo encuentra, lanza un error
            {
                throw new HttpError($"No se encontro el Estado con ID = {id}", HttpStatusCode.NotFound);
            }

            return estado;
        }

        // Devuelve todos los estados en forma de lista asincrónicamente.
        public async Task<List<Estado>> GetAll()
        {
            var estados = await _db.Estados.ToListAsync();
            return estados;
        }

        // Método público que utiliza el método privado GetOneByIdOrException() para obtener un estado por su id.
        public async Task<Estado> GetOneById(int id)
        {
            return await GetOneByIdOrException(id); //Si no lo encuentra, lanza excepción automáticamente.
        }

        // Busca un Estado por su Nombre
        public async Task<Estado> GetOneByName(string nombre)
        {
            var estado = await _db.Estados.FirstOrDefaultAsync(e => e.nombreEstado == nombre);
            if (estado == null) //Si no lo encuentra, lanza una excepción 404 con mensaje específico.
            {
                throw new HttpError($"No se encontro el Estado con Nombre = {nombre}", HttpStatusCode.NotFound);
            }
            return estado;
        }
    }
}
