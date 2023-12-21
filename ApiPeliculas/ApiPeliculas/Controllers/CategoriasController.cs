using ApiPeliculas.Modelos;
using ApiPeliculas.Modelos.Dtos;
using ApiPeliculas.Repositorio.IRepositorio;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace ApiPeliculas.Controllers
{
    [ApiController]
    //[Route("api/[controller]")] /// Una opcion
    [Route("api/categorias")]
    public class CategoriasController : ControllerBase // Controlador Api
    {
        private readonly ICategoriaRepositorio _ctRepo;
        private readonly IMapper _mapper;

        public CategoriasController(ICategoriaRepositorio ctRepo, IMapper mapper)
        {
            _ctRepo = ctRepo;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetCategorias() {
            var listaCategorias = _ctRepo.GetCategorias();

            var listaCategoriasDTO = new List<CategoriaDTO>();

            foreach (var lista in listaCategorias)
            {
                listaCategoriasDTO.Add(_mapper.Map<CategoriaDTO>(lista)); // tranforma el objeto categoria normal con todos sus items a 
            }                                                            // el objeto con solo los items definidos en categoriaDTO y lo agrega asi a la lista
            return Ok(listaCategoriasDTO);
        }

        [HttpGet("{categoriaId:int}", Name = "GetCategoria")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetCategoria(int categoriaId)
        {
            var itemCategoria = _ctRepo.GetCategoria(categoriaId);



            if (itemCategoria == null)
            {
                return NotFound();
            }

            var itemCategotiaDTO = _mapper.Map<CategoriaDTO>(itemCategoria); // tranforma el objeto categoria normal con todos sus items a 
                                                                             // el objeto con solo los items definidos en categoriaDTO
            return Ok(itemCategotiaDTO);
        }

        [HttpPost]
        [ProducesResponseType(201, Type = typeof(CategoriaDTO))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public IActionResult CrearCategoria([FromBody] CrearCategoriaDTO crearCategoriaDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (crearCategoriaDTO == null)
            {
                return BadRequest(ModelState);
            }
            if (_ctRepo.ExixteCategoria(crearCategoriaDTO.Nombre))
            {
                ModelState.AddModelError("", "La categoria ya existe");
                return StatusCode(404, ModelState);
            }
            var categoria = _mapper.Map<Categoria>(crearCategoriaDTO);  /// 

            if (!_ctRepo.CrearCategoria(categoria))
            {
                ModelState.AddModelError("", $"Algo salio mal el registro{categoria.Nombre}");
                return StatusCode(500, ModelState);
            }
            return CreatedAtRoute("GetCategoria", new { categoriaId = categoria.Id }, categoria);
        }

        [HttpPatch("{categoriaId:int}", Name = "ActualizarPatchCategoria")]
        [ProducesResponseType(201, Type = typeof(CategoriaDTO))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult ActualizarPatchCategoria(int categoriaId, [FromBody] CategoriaDTO categoriaDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (categoriaDto == null || categoriaId != categoriaDto.Id)
            {
                return BadRequest(ModelState);
            }


            var categoria = _mapper.Map<Categoria>(categoriaDto);
            if (!_ctRepo.ActualizarCategoria(categoria))
            {
                ModelState.AddModelError("", $"Algo salio mal actualizando el registro{categoria.Nombre}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpDelete("{categoriaId:int}", Name = "BorrarCategoria")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult BorrarCategoria(int categoriaId)
        {
           if(! _ctRepo.ExixteCategoria(categoriaId))
            {
                return NotFound();
            }
            var categoria = _ctRepo.GetCategoria(categoriaId);
            
            if (!_ctRepo.BorrarCategoria(categoria))
            {
                ModelState.AddModelError("", $"Algo salio mal borrando el registro{categoria.Nombre}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}
