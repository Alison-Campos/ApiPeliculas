
using ApiPeliculas.Modelos;
using ApiPeliculas.Modelos.Dtos;
using ApiPeliculas.Repositorio.IRepositorio;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace ApiPeliculas.Controllers
{
    [ApiController]
    [Route("api/peliculas")]
    public class PeliculaController : ControllerBase
    {
        private readonly IPeliculaRepositorio _ctRepo;
        private readonly IMapper _mapper;
        private readonly ICategoriaRepositorio _ctRepo2;

        public PeliculaController(IPeliculaRepositorio ctRepo, IMapper mapper, ICategoriaRepositorio ctRepo2)
        {
            _ctRepo = ctRepo;
            _mapper = mapper;
            _ctRepo2 = ctRepo2;

        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public IActionResult GetPeliculas() 
        {
            var listaPeliculas = _ctRepo.GetPeliculas();
            var listaPeliculasDTO = new List<PeliculaDTO>();

            foreach(var lista in listaPeliculas)
            {
                listaPeliculasDTO.Add(_mapper.Map<PeliculaDTO>(lista));
            }
            return Ok(listaPeliculasDTO);
        }

        [HttpGet("{peliculaId:int}", Name = "GetPelicula")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetPelicula(int peliculaId)
        {
            var itemPelicula = _ctRepo.GetPelicula(peliculaId);

            if (itemPelicula == null)
            {
                return NotFound();
            }

            var itemPeliculaDTO = _mapper.Map<PeliculaDTO>(itemPelicula); 
                                                                            
            return Ok(itemPeliculaDTO);
        }

        [HttpPost]
        [ProducesResponseType(201, Type = typeof(PeliculaDTO))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public IActionResult CrearPelicula([FromBody ] PeliculaDTO peliculaDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if ( peliculaDTO == null)
            {
                return BadRequest(ModelState);
            }
            if (_ctRepo.ExistePelicula(peliculaDTO.Name))
            {
                ModelState.AddModelError("", "La pelicula ya existe");
                return StatusCode(404, ModelState);
            }
            if(!_ctRepo2.ExixteCategoria(peliculaDTO.categoriaId))
            {
                ModelState.AddModelError("", "La categoria no existe");
                return StatusCode(404, ModelState);
            }
            
            var pelicula = _mapper.Map<Pelicula>(peliculaDTO);
                //pelicula.Categoria = _ctRepo2.GetCategoria(peliculaDTO.categoriaId);
               
          if (!_ctRepo.CrearPelicula(pelicula))
             {
                 ModelState.AddModelError("", $"Algo salio mal el registro{pelicula.Name}");
                return StatusCode(500, ModelState);
             }
            return CreatedAtRoute("GetPelicula", new { peliculaId = pelicula.Id }, pelicula);
            
        }
        [HttpPatch("{peliculaId:int}", Name = "ActualizarPatchPelicula")]
        [ProducesResponseType(201, Type = typeof(CategoriaDTO))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult ActualizarPatchPelicula(int peliculaId, [FromBody] PeliculaDTO peliculaDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (peliculaDto == null || peliculaId != peliculaDto.Id)
            {
                return BadRequest(ModelState);
            }


            var pelicula = _mapper.Map<Pelicula>(peliculaDto);
           
            if(_ctRepo.ExistePelicula(peliculaDto.Name)) ///
            {
                ModelState.AddModelError("", "Ya existe una pelicula con este nombre");
                return StatusCode(404, ModelState);
            }
            if (!_ctRepo2.ExixteCategoria(pelicula.categoriaId))
            {
                ModelState.AddModelError("", "No existe esta categoria, digita otra");
                return StatusCode(404, ModelState);
            }
            if (!_ctRepo.ActualizarPelicula(pelicula))
            {
                ModelState.AddModelError("", $"Algo salio mal actualizando el registro{pelicula.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpDelete("{peliculaId:int}", Name = "BorrarPelicula")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult BorrarPelicula(int peliculaId)
        {
            if (!_ctRepo.ExistePelicula(peliculaId))
            {
                return NotFound();
            }
            var pelicula = _ctRepo.GetPelicula(peliculaId);

            if (!_ctRepo.BorrarPelicula(pelicula))
            {
                ModelState.AddModelError("", $"Algo salio mal borrando el registro{pelicula.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

    }
}
