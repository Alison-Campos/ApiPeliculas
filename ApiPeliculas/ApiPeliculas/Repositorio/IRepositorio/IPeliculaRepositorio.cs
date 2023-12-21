using ApiPeliculas.Modelos;

namespace ApiPeliculas.Repositorio.IRepositorio
{
    public interface IPeliculaRepositorio
    {
        ICollection<Pelicula> GetPeliculas();  //devuelve una colección de objetos del tipo Pelicula
        Pelicula GetPelicula(int PeliculaId);
        bool ExistePelicula(string nombre);
        bool ExistePelicula (int id);
        bool CrearPelicula(Pelicula pelicula);
        bool ActualizarPelicula(Pelicula pelicula);
        bool BorrarPelicula(Pelicula pelicula);
        bool Guardar();


    }
}
