using HeladeriaAPI.Models.Helado;
using HeladeriaAPI.Models.Ingrediente;
using HeladeriaAPI.Models.Ingrediente.Dto;
using HeladeriaAPI.Services;
using HeladeriaAPI.Utils;
using Microsoft.AspNetCore.Mvc;

namespace HeladeriaAPI.Controllers
{
    [Route("api/ingredientes")]//Define que el controlador responderá a rutas que comiencen con /api/ingredientes.
    [ApiController]// habilita comportamientos automáticos como la validación del modelo y respuestas 400 automáticas si fallan.
    public class IngredienteController : ControllerBase//Declara el controlador como público y hereda de ControllerBase, la clase base recomendada para APIs RESTful (sin vistas).
    {
        //Campo privado para el servicio de ingredientes que se inyectará por dependencia.
        private readonly IngredienteServices _ingredienteServices;

        //Constructor que recibe la dependencia del servicio y la asigna al campo privado.Esto permite la inversión de dependencias(DI).
        public IngredienteController(IngredienteServices ingredienteServices)
        {
            _ingredienteServices = ingredienteServices;
        }

        [HttpGet]//Este método responde a GET /api/ingredientes
        //Devuelve 200 OK si todo va bien, y 500 en caso de error interno, con un objeto HttpMessage.
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(HttpMessage))]

        //Método asincrónico que retorna una lista de objetos Ingrediente dentro de un ActionResult.
        public async Task<ActionResult<List<Ingrediente>>> GetIngredientes()
        {
            try //Intenta obtener todos los ingredientes usando el servicio.
            {
                var ingredientes = await _ingredienteServices.GetAll();
                return Ok(ingredientes);//Si tiene éxito, devuelve 200 OK con la lista.
            }
            catch (HttpError ex) //Captura un HttpError personalizado
            {
                return StatusCode((int)ex.StatusCode, new HttpMessage(ex.Message));
            }
            catch //Cualquier otro error general devuelve 500 con mensaje genérico
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new HttpMessage("Algo salio mal"));
            }
        }

        //Método comentado: [HttpPost("/ids")]
        //Comentado, pero serviría para obtener ingredientes por IDs enviados en el body.
        //Devuelve una lista de ingredientes específica según IDs.
        //❗ Posiblemente útil, aunque es más común recibir arrays por GET con query strings (?ids=1&ids=2), pero eso depende del caso.


        //[HttpPost("/ids")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(HttpMessage))]
        //public async Task<ActionResult<List<Ingrediente>>> GetIngredientesIds([FromBody]List<int> ids)
        //{
        //    try
        //    {
        //        var ingredientes = await _ingredienteServices.GetAllByIds(ids);
        //        return Ok(ingredientes);
        //    }
        //    catch(HttpError ex)
        //    {
        //        return StatusCode((int)ex.StatusCode, new HttpMessage(ex.Message));
        //    }
        //    catch
        //    {
        //        return StatusCode(StatusCodes.Status500InternalServerError, new HttpMessage("Algo salio mal"));
        //    }
        //}

        [HttpGet("{id}")]//Responde a GET /api/ingredientes/{id}.
        //Devuelve 200 si lo encuentra, 404 si no, y 500 si hay un error no controlado.
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(HttpMessage))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(HttpMessage))]

        //Método asincrónico que devuelve un solo Ingrediente.
        public async Task<ActionResult<Ingrediente>> GetIngrediente(int id)
        {
            try
            {
                return await _ingredienteServices.GetOneById(id); //Retorna el ingrediente directamente si lo encuentra.
                // Es una práctica válida devolver directamente un objeto con ActionResult<T>, aunque si querés más control de respuesta (por ejemplo NotFound()), deberías usar return Ok(obj) o return NotFound().
            }
            //Manejo de errores igual que el anterior.
            catch (HttpError ex)
            {
                return StatusCode((int)ex.StatusCode, new HttpMessage(ex.Message));
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new HttpMessage("Algo salio mal"));
            }
        }

        [HttpPost]//Este método responde a POST /api/ingredientes.
        //Devuelve 201 si se creó, 400 si hay errores de validación, 500 para otros errores.
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Helado))] //Hay una inconsistencia en el tipo de respuesta, dice que devuelve un Helado cuando en realidad está creando un Ingrediente
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationErrorResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(HttpMessage))]

        //Método que recibe un DTO con los datos del nuevo ingrediente.
        //Usa CreateIngredienteDTO, lo cual es correcto y seguro (evita que el usuario envíe campos no deseados).
        public async Task<ActionResult> Create([FromBody] CreateIngredienteDTO ingrediente)
        {
            try
            {
                var ingredienteCreated = await _ingredienteServices.CreateOne(ingrediente);//Crea el ingrediente usando el servicio.
                return Created("api/helados", ingredienteCreated); //Devuelve 201 con el objeto creado.
                //El Created("api/helados", ...) también parece un error. Debería ser "api/ingredientes" o CreatedAtAction(...) para indicar dónde se puede obtener el nuevo recurso.
            }
            //Mismo esquema de manejo de errores.
            catch (HttpError ex)
            {
                return StatusCode((int)ex.StatusCode, new HttpMessage(ex.Message));
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new HttpMessage("Algo salio mal creando el Ingrediente"));
            }
        }

        [HttpPut("{id}")]//Responde a PUT /api/ingredientes/{id}.
        //Devuelve 200 si se actualiza, 404 si no se encuentra, 400 por errores de validación, 500 por errores internos.
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Helado))] // De nuevo, inconsistencia en el tipo: dice que devuelve Helado, debería ser Ingrediente.
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(HttpMessage))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationErrorResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(HttpMessage))]
        //Llama al método del servicio para actualizar por ID.
        public async Task<ActionResult<Ingrediente>> Update(int id, [FromBody] UpdateIngredienteDTO ingrediente)
        {
            try
            {
                return await _ingredienteServices.UpdateOneById(id, ingrediente);//Devuelve el ingrediente actualizado.
            }
            //Mismo esquema de manejo de errores.
            catch (HttpError ex)
            {
                return StatusCode((int)ex.StatusCode, new HttpMessage(ex.Message));
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new HttpMessage($"Algo salio mal actualizando el Ingrediente con ID = {id}"));
            }
        }

        [HttpDelete("{id}")]//Maneja la ruta DELETE /api/ingredientes/{id}.
        //Devuelve 200 si se elimina correctamente, 404 si no lo encuentra.
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(HttpMessage))]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                await _ingredienteServices.DeleteOneById(id);//Llama al servicio para eliminar.
                return Ok(new HttpMessage($"Ingrediente con ID = {id} eliminado correctamente."));//Devuelve mensaje de confirmación como HttpMessage.
            }
            //Manejo de errores igual a los otros métodos.
            catch (HttpError ex)
            {
                return StatusCode((int)ex.StatusCode, new HttpMessage(ex.Message));
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new HttpMessage($"Algo salio mal eliminando el Ingrediente con ID = {id}"));
            }
        }
    }
}
