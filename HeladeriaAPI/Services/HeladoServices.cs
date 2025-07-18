﻿using System.Net;
using AutoMapper;
using HeladeriaAPI.Config;
using HeladeriaAPI.Models.Helado;
using HeladeriaAPI.Models.Helado.Dto;
using HeladeriaAPI.Models.Ingrediente;
using HeladeriaAPI.Models.Ingrediente.Dto;
using HeladeriaAPI.Utils;
using Microsoft.EntityFrameworkCore;

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

        public async Task<AllIngredienteHeladoDTO?> GetIngredientesDeHelado(int heladoId)
        {
            var helado = await _db.Helados
                .Include(h => h.IngredienteHelado)
                    .ThenInclude(ih => ih.Ingrediente)
                .FirstOrDefaultAsync(h => h.Id == heladoId);

            if (helado == null) return null;

            return new AllIngredienteHeladoDTO
            {
                HeladoId = helado.Id,
                nombreHelado = helado.nombreHelado,
                Ingredientes = helado.IngredienteHelado.Select(ih => new IngredienteDTO
                {
                    IngredienteId = ih.IngredienteId,
                    nombreIngrediente = ih.Ingrediente.nombreIngrediente
                }).ToList()
            };
        }



        // Busca un helado por ID incluyendo sus relaciones (Estado e Ingredientes).
        private async Task<Helado> GetOneByIdOrException(int id)
        {
            var helado = await _db.Helados
                .Where(h => h.Id == id)
                .Include(h => h.Categoria)
                .Include(h => h.Estado)
                .Include(h => h.IngredienteHelado)
                    .ThenInclude(ih => ih.Ingrediente)
                .FirstOrDefaultAsync();

            if (helado == null)
            {
                throw new HttpError($"No se encontró el helado con ID = {id}", HttpStatusCode.NotFound);
            }

            return helado;
        }


        //Devuelve una lista de todos los helados con su estado.
        // Mapea a DTOs (AllHeladoDTO) para respuesta liviana y estructurada.
        public async Task<List<AllHeladoDTO>> GetAll()
        {
            var heladosDb = await _db.Helados
                .Include(hel => hel.Estado)
                .Include(cat => cat.Categoria)
                .Include(h => h.IngredienteHelado)
                .ThenInclude(ih => ih.Ingrediente)
                .ToListAsync();
            var helados = _mapper.Map<List<AllHeladoDTO>>(heladosDb);
            return helados;
        }

        //Expone públicamente la lógica interna de búsqueda por ID.
        public async Task<AllHeladoDTO> GetOneByIdDTO(int id)
        {
            var helado = await GetOneByIdOrException(id);
            return _mapper.Map<AllHeladoDTO>(helado);
        }

        //Crea un helado nuevo a partir del DTO.
        //Detalles importantes:
        //Asigna automáticamente el estado "Pendiente" usando EstadoServices.
        //Agrega el ingrediente "default" como mínimo inicial.
        // Esto asegura que ningún helado quede sin estado ni ingredientes.
        public async Task<Helado> CreateOne(CreateHeladoDTO heladoDto)
        {
            var h = _mapper.Map<Helado>(heladoDto);

            // Buscar estado
            var estado = await _db.Estados.FindAsync(heladoDto.EstadoId);
            if (estado == null)
                throw new HttpError($"Estado con ID = {heladoDto.EstadoId} no encontrado", HttpStatusCode.BadRequest);
            h.Estado = estado;

            // Buscar categoría
            var categoria = await _db.Categorias.FindAsync(heladoDto.CategoriaId);
            if (categoria == null)
                throw new HttpError($"Categoría con ID = {heladoDto.CategoriaId} no encontrada", HttpStatusCode.BadRequest);
            h.Categoria = categoria;

            // Validar ingredientes
            if (heladoDto.IngredientesIds == null || !heladoDto.IngredientesIds.Any())
                throw new HttpError("Debe especificar al menos un ingrediente.", HttpStatusCode.BadRequest);

            var ingredientesValidos = await _db.Ingredientes
                .Where(i => heladoDto.IngredientesIds.Contains(i.Id))
                .ToListAsync();

            var ingredientesInvalidos = heladoDto.IngredientesIds.Except(ingredientesValidos.Select(i => i.Id)).ToList();
            if (ingredientesInvalidos.Any())
                throw new HttpError($"Los siguientes ingredientes no existen: {string.Join(", ", ingredientesInvalidos)}", HttpStatusCode.BadRequest);

            // Crear relaciones muchos a muchos
            h.IngredienteHelado = heladoDto.IngredientesIds.Select(id => new IngredienteHelado
            {
                IngredienteId = id
            }).ToList();

            h.FechaCreacion = DateTime.UtcNow;

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

            // Actualizar campos básicos si fueron enviados
            if (helado.nombreHelado != null) heladoToUpdate.nombreHelado = helado.nombreHelado;
            if (helado.Descripcion != null) heladoToUpdate.Descripcion = helado.Descripcion;
            if (helado.Precio.HasValue) heladoToUpdate.Precio = helado.Precio.Value;
            if (helado.IsArtesanal.HasValue) heladoToUpdate.IsArtesanal = helado.IsArtesanal.Value;
            if (helado.CategoriaId.HasValue) heladoToUpdate.CategoriaId = helado.CategoriaId.Value;
            if (helado.EstadoId.HasValue) heladoToUpdate.EstadoId = helado.EstadoId.Value;

            // Agregar nuevos ingredientes sin eliminar los actuales
            if (helado.IngredientesIds != null)
            {
                var actuales = await _db.IngredienteHelado
                    .Where(ih => ih.HeladoId == heladoToUpdate.Id)
                    .ToListAsync();

                _db.IngredienteHelado.RemoveRange(actuales);

                heladoToUpdate.IngredienteHelado = new List<IngredienteHelado>();

                foreach (var nuevoId in helado.IngredientesIds)
                {
                    heladoToUpdate.IngredienteHelado.Add(new IngredienteHelado
                    {
                        HeladoId = heladoToUpdate.Id,
                        IngredienteId = nuevoId
                    });
                }
            }
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

        public async Task AddIngredienteToHelado(int heladoId, int ingredienteId)
        {
            // Verificar si el helado existe
            var helado = await _db.Helados.FindAsync(heladoId);
            if (helado == null)
                throw new HttpError($"No se encontró el helado con ID = {heladoId}", HttpStatusCode.NotFound);

            // Verificar si el ingrediente existe
            var ingrediente = await _db.Ingredientes.FindAsync(ingredienteId);
            if (ingrediente == null)
                throw new HttpError($"Ingrediente con ID = {ingredienteId} no encontrado", HttpStatusCode.BadRequest);

            // Verificar si ya existe la relación
            var existe = await _db.IngredienteHelado
                .AnyAsync(ih => ih.HeladoId == heladoId && ih.IngredienteId == ingredienteId);

            if (existe)
                throw new HttpError("Ese ingrediente ya está asociado al helado.", HttpStatusCode.BadRequest);

            // Crear relación
            var ingredienteHelado = new IngredienteHelado
            {
                HeladoId = heladoId,
                IngredienteId = ingredienteId
            };

            await _db.IngredienteHelado.AddAsync(ingredienteHelado);

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw new HttpError("Error inesperado al guardar el ingrediente al helado.", HttpStatusCode.InternalServerError);
            }
        }

        public async Task RemoveIngredienteFromHelado(int heladoId, int ingredienteId)
        {
            var relacion = await _db.IngredienteHelado
                .FirstOrDefaultAsync(ih => ih.HeladoId == heladoId && ih.IngredienteId == ingredienteId);

            if (relacion == null)
                throw new HttpError("No existe esa relación entre helado e ingrediente.", HttpStatusCode.NotFound);

            _db.IngredienteHelado.Remove(relacion);

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw new HttpError("Error inesperado al eliminar el ingrediente del helado.", HttpStatusCode.InternalServerError);
            }
        }
    }
}
