using HeladeriaAPI.Models.Helado;
using HeladeriaAPI.Models.Helado.Dto;
using HeladeriaAPI.Services;
using HeladeriaAPI.Utils;
using Microsoft.AspNetCore.Mvc; // es fundamental para usar [ApiController] y otros atributos.

namespace HeladeriaAPI.Controllers
{
    [Route("api/helados")] //Define la ruta base para este controlador.
    [ApiController]//Habilita validación automática de modelos, inferencia de parámetros, entre otras funciones útiles.
    public class HeladoController : ControllerBase //ControllerBase, Clase base de controladores que no devuelven vistas, sólo JSON (ideal para APIs REST).
    {
        //Se declara una dependencia al servicio de helados, que será inyectada mediante constructor. Permite delegar la lógica de negocio fuera del controlador.
        private readonly HeladoServices _heladoServices;
        // Constructor que implementa inyección de dependencias: el servicio se inyecta y se asigna a un campo privado.
        public HeladoController(HeladoServices heladoServices)
        {
            _heladoServices = heladoServices;
        }

        [HttpGet]//Responde a solicitudes GET a api/helados.
        //Retorna una lista de DTOs (no entidades completas), mapeados previamente en el servicio.
        public async Task<List<AllHeladoDTO>> GetHelados()
        {
            return await _heladoServices.GetAll();
        }

        //Este endpoint busca un helado por ID (GET api/helados/{id}).
        [HttpGet("{id}")]
        //Se documentan los posibles códigos de respuesta:
        [ProducesResponseType(StatusCodes.Status200OK)] //200 OK si se encuentra
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(HttpMessage))] //404 si no se encuentra
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(HttpMessage))] //500 en caso de error interno.
        public async Task<ActionResult<AllHeladoDTO>> GetHelado(int id)
        {
            try
            {
                return await _heladoServices.GetOneByIdDTO(id);
            }
            catch (HttpError ex) //Se maneja la excepción personalizada HttpError, permitiendo devolver el código y mensaje específico.
            {
                return StatusCode((int)ex.StatusCode, new HttpMessage(ex.Message));
            }
            catch //También se captura cualquier otro error general devolviendo código 500.
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new HttpMessage("Algo salio mal"));
            }
        }

        [HttpPost]//Crea un nuevo helado (POST api/helados).
        // Se especifican los posibles códigos de respuesta:
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Helado))]//201 Created si se crea correctamente
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationErrorResponse))]//400 BadRequest si hay errores de validación
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(HttpMessage))]//500 Internal Server Error si hay fallas internas
        public async Task<ActionResult> Create([FromBody] CreateHeladoDTO helado)//Se llama al método de creación del servicio.
        {
            try
            {
                var heladoCreated = await _heladoServices.CreateOne(helado);
                return Created("api/helados", heladoCreated);// Created() devuelve código 201 y el objeto creado.
            }
            catch (HttpError ex) //Se manejan errores con HttpError y excepciones generales.
            {
                return StatusCode((int)ex.StatusCode, new HttpMessage(ex.Message));
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new HttpMessage("Algo salio mal creando el helado"));
            }
        }

        [HttpPut("{id}")]//Endpoint para actualizar un helado existente (PUT api/helados/{id}).
        //Se documentan los posibles códigos
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Helado))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(HttpMessage))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationErrorResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(HttpMessage))]
        public async Task<ActionResult<Helado>> Update(int id, [FromBody] UpdateHeladoDTO helado) //Usa UpdateHeladoDTO como entrada. Se delega la lógica al servicio.
        {
            try // Se devuelve el helado actualizado o un mensaje de error según el caso.
            {
                return await _heladoServices.UpdateOneById(id, helado);
            }
            catch (HttpError ex)
            {
                return StatusCode((int)ex.StatusCode, new HttpMessage(ex.Message));
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new HttpMessage($"Algo salio mal actualizando el helado con ID = {id}"));
            }
        }

        [HttpDelete("{id}")]//Elimina un helado por ID (DELETE api/helados/{id}).
        //Documenta los posibles códigos de salida: 200 (ok) y 404 (no encontrado).
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(HttpMessage))]
        public async Task<ActionResult> Delete(int id)
        {
            try //Ejecuta la eliminación.
            {
                await _heladoServices.DeleteOneById(id);
                return Ok(new HttpMessage($"Helado con ID = {id} eliminado correctamente.")); //Si todo sale bien, responde con mensaje de confirmación y status 200.
            }
            catch (HttpError ex) // Maneja errores con HttpError o error general.
            {
                return StatusCode((int)ex.StatusCode, new HttpMessage(ex.Message));
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new HttpMessage($"Algo salio mal eliminando el helado con ID = {id}"));
            }
        }
    }
}
