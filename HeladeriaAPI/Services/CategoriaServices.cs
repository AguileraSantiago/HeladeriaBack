using System.Net;
using AutoMapper;
using HeladeriaAPI.Config;
using HeladeriaAPI.Models.Categoria;
using HeladeriaAPI.Models.Categoria.Dto;
using HeladeriaAPI.Utils;
using Microsoft.EntityFrameworkCore;

namespace HeladeriaAPI.Services
{
    public class CategoriaServices
    {
        public readonly IMapper _mapper;
        private readonly ApplicationDbContext _db;
        public CategoriaServices(IMapper mapper, ApplicationDbContext db)
        {
            _mapper = mapper;
            _db = db;
        }

        private async Task<Categoria> GetOneByIdOrException(int id)
        {
            var categoria = await _db.Categorias
                .Include(c => c.Helados)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (categoria == null)
                throw new HttpError($"No se encontró la categoría con ID = {id}", HttpStatusCode.NotFound);


            return categoria;
        }


        public async Task<List<AllCategoriaDTO>> GetAll()
        {
            var categorias = await _db.Categorias
                .Include(c => c.Helados) // 👈 Esto trae los helados relacionados
                .ToListAsync();

            return _mapper.Map<List<AllCategoriaDTO>>(categorias);
        }


        public async Task<Categoria> GetOneById(int id)
        {
            return await GetOneByIdOrException(id);
        }


        public async Task<Categoria> CreateOne(CreateCategoriaDTO dto)
        {
            var nuevaCategoria = _mapper.Map<Categoria>(dto);

            await _db.Categorias.AddAsync(nuevaCategoria);
            await _db.SaveChangesAsync();

            return nuevaCategoria;
        }


        public async Task<Categoria> UpdateOneById(int id, UpdateCategoriaDTO dto)
        {
            var categoriaUpdate = await GetOneByIdOrException(id);
            _mapper.Map(dto, categoriaUpdate);

            _db.Categorias.Update(categoriaUpdate);
            await _db.SaveChangesAsync();

            return categoriaUpdate;
        }

        public async Task DeleteOneById(int id)
        {
            var categoria = await GetOneByIdOrException(id);
            _db.Categorias.Remove(categoria);
            await _db.SaveChangesAsync();

            if (await _db.Categorias.AnyAsync(c => c.Id == id))
            {
                throw new HttpError($"No se pudo eliminar la categoria con ID = {id}", HttpStatusCode.InternalServerError);  
            }
        }

    }
}
