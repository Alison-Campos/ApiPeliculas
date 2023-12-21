using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiPeliculas.Modelos.Dtos
{
    public class PeliculaDTO
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="El nombre es obligatorio")]
        public string Name { get; set; }

        public string RutaImagen { get; set; }
        [Required(ErrorMessage = "La ruta de imagen es obligatorio")]

        public string Descripcion { get; set; }
        [Required(ErrorMessage = "La descripcion es obligatorio")]

        public int Duracion { get; set; }

        public enum TipoClasificacion { Siete, Trece, Dieciseis, Dieciocho }
        public TipoClasificacion Clasificacion { get; set; }
        public DateTime FechaCreacion { get; set; }

      
        public int categoriaId { get; set; }

       
    }
}
