using ApiPeliculas.Modelos;
using ApiPeliculas.Modelos.Dtos;
using ApiPeliculas.Repositorio.IRepositorio;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ApiPeliculas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioRepositorio _usRepo;
        protected RespuestaApi _respuestaApi;
        private readonly IMapper _mapper;
        public UsuariosController(IUsuarioRepositorio usRepo, IMapper mapper)
        {
            _usRepo = usRepo;
            this._respuestaApi = new();
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetUsuarios()
        {
            var listaUsuarios = _usRepo.GetUsuarios();

            var listaUsuariosDto = new List<UsuarioDTO>();

            foreach (var lista in listaUsuarios)
            {
                listaUsuariosDto.Add(_mapper.Map<UsuarioDTO>(lista));
            }
            return Ok(listaUsuariosDto);


        }

        [HttpGet("{usuarioId:int}", Name = "GetUsuario")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetUsuario(int usuarioId)
        {
            var itemUsuario= _usRepo.GetUsuario(usuarioId);



            if (itemUsuario == null)
            {
                return NotFound();
            }

            var itemUsuarioDTO = _mapper.Map<UsuarioDTO>(itemUsuario); // tranforma el objeto categoria normal con todos sus items a 
                                                                             // el objeto con solo los items definidos en categoriaDTO
            return Ok(itemUsuarioDTO);
        }

        [HttpPost("registro")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> Registro([FromBody] UsuarioRegistroDTO usuarioRegistrroDTO)
        {
            bool validarNombreUsuarioUnico = _usRepo.IsUniqueUser(usuarioRegistrroDTO.NombreUsuario);
            if(!validarNombreUsuarioUnico) 
            {
                _respuestaApi.StatusCode = HttpStatusCode.BadRequest;
                _respuestaApi.IsSuccess= false;
                _respuestaApi.ErrorMessages.Add("El nombre de usuario ya existe");
                return BadRequest();
            }

            var usuario = await _usRepo.Registro(usuarioRegistrroDTO);
            if(usuario== null) 
            {
                _respuestaApi.StatusCode = HttpStatusCode.BadRequest;
                _respuestaApi.IsSuccess = false;
                _respuestaApi.ErrorMessages.Add("Erro en el registro");
                return BadRequest();

            }
            _respuestaApi.StatusCode = HttpStatusCode.OK;
            _respuestaApi.IsSuccess = true;
            return Ok(_respuestaApi);
        }
    }
}
