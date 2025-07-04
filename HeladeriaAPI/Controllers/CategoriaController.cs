using HeladeriaAPI.Models.Categoria;
using HeladeriaAPI.Models.Categoria.Dto;
using HeladeriaAPI.Models.Helado;
using HeladeriaAPI.Models.Helado.Dto;
using HeladeriaAPI.Services;
using HeladeriaAPI.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace HeladeriaAPI.Controllers
{
    [Route("api/categorias")]
    [ApiController]
    public class CategoriaController : ControllerBase
    {
        private readonly CategoriaServices _categoriaServices;

        public CategoriaController(CategoriaServices categoriaServices)
        {
            _categoriaServices = categoriaServices;
        }

        [HttpGet]
        public async Task<List<AllCategoriaDTO>> GetCategorias()
        {
            return await _categoriaServices.GetAll();
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)] 
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(HttpMessage))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(HttpMessage))]

        public async Task<ActionResult<Categoria>> GetCategoria(int id)
        {
            try
            {
                var categoriaDTO = await _categoriaServices.GetOneByIdDTO(id);
                return Ok(categoriaDTO);
            }
            catch (HttpError ex)
            {
                return StatusCode((int)ex.StatusCode, new HttpMessage(ex.Message));
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new HttpMessage("Algo salió mal"));
            }
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Categoria))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationErrorResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(HttpMessage))]


        public async Task<ActionResult> Create([FromBody] CreateCategoriaDTO categoria)//Se llama al método de creación del servicio.
        {
            try
            {
                var categoriaCreated = await _categoriaServices.CreateOne(categoria);
                return Created("api/categorias", categoriaCreated);// Created() devuelve código 201 y el objeto creado.
            }
            catch (HttpError ex) //Se manejan errores con HttpError y excepciones generales.
            {
                return StatusCode((int)ex.StatusCode, new HttpMessage(ex.Message));
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new HttpMessage("Algo salio mal creando la categoria"));
            }
        }


        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Categoria))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(HttpMessage))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationErrorResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(HttpMessage))]
        public async Task<ActionResult<Categoria>> Update(int id, [FromBody] UpdateCategoriaDTO categoria) 
        {
            try 
            {
                return await _categoriaServices.UpdateOneById(id, categoria);
            }
            catch (HttpError ex)
            {
                return StatusCode((int)ex.StatusCode, new HttpMessage(ex.Message));
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new HttpMessage($"Algo salio mal actualizando la categoria con ID = {id}"));
            }
        }



        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(HttpMessage))]
        public async Task<ActionResult> Delete(int id)
        {
            try 
            {
                await _categoriaServices.DeleteOneById(id);
                return Ok(new HttpMessage($"Categoria con ID = {id} eliminada correctamente."));
            }
            catch (HttpError ex) // Maneja errores con HttpError o error general.
            {
                return StatusCode((int)ex.StatusCode, new HttpMessage(ex.Message));
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new HttpMessage($"Algo salio mal eliminando la categoria con ID = {id}"));
            }
        }



    }
}
