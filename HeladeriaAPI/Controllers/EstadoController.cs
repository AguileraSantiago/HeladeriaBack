using HeladeriaAPI.Models.Estado;
using HeladeriaAPI.Models.Estado.Dto;
using HeladeriaAPI.Services;
using HeladeriaAPI.Utils;
using Microsoft.AspNetCore.Mvc;
namespace HeladeriaAPI.Controllers
{
    [Route("api/estados")]
    [ApiController]
    public class EstadoController : ControllerBase
    {
        private readonly EstadoServices _estadoServices;

        public EstadoController(EstadoServices estadoServices)
        {
            _estadoServices = estadoServices;
        }


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<AllEstadoDTO>>> GetAllEstados()
        {
            var estados = await _estadoServices.GetAll();
            return Ok(estados);
        }


        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(HttpMessage))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(HttpMessage))]
        public async Task<ActionResult<Estado>> GetEstadoById(int id)
        {
            try
            {
                var estado = await _estadoServices.GetOneById(id);
                return Ok(estado);
            }
            catch (HttpError ex)
            {
                return StatusCode((int)ex.StatusCode, new HttpMessage(ex.Message));
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new HttpMessage($"Error interno al obtener el estado con ID: {id}."));
            }
        }
    }
}