using ApiPeliculas.Data;
using ApiPeliculas.Modelos;
using ApiPeliculas.Repositorio.IRepositorio;

namespace ApiPeliculas.Repositorio
{
    public class CategoriaRepositorio : ICategoriaRepositorio
    {
        private readonly AplicationDbContext _bd; // readonly: indica que el valor de la variable solo puede asignarse dentro del constructor de la clase. 

       
        public CategoriaRepositorio(AplicationDbContext bd)
        {
            _bd = bd;
        }
        public bool ActualizarCategoria(Categoria categoria)
        {
            categoria.FechaCreacion = DateTime.Now;// acceda al objeto y en el campo de fecha creacion ponga la de este momento
            _bd.Categoria.Update(categoria); // actualice la tabla  categoria de la bd con ese nuevo dato
            return Guardar();
        }

        public bool BorrarCategoria(Categoria categoria)
        {
            _bd.Categoria.Remove(categoria);
            return Guardar ();
        }

        public bool CrearCategoria(Categoria categoria)
        {
            categoria.FechaCreacion = DateTime.Now; // acceda al objeto y en el campo de fecha creacion ponga la de este momento
            _bd.Categoria.Add(categoria); // agregue a la tabla categoria de la bd la nueva categoria
            return Guardar ();
        }

        public bool ExixteCategoria(string nombre)
        {
            bool valor =  _bd.Categoria.Any(c => c.Nombre.ToLower().Trim() == nombre.ToLower().Trim()); // true si existe un nombre en la tabla de la bd que
            return valor;                                                                               //  coincida con el nombre digitado ignore mayusculas
                                                                                                        // (toLower ignora mayusculas) y espacios(Trim)             
        }

        public bool ExixteCategoria(int id)
        {
            return _bd.Categoria.Any(c => c.Id == id);
        }

        public Categoria GetCategoria(int categoriaId)
        {
            return _bd.Categoria.FirstOrDefault(c => c.Id == categoriaId);
        }

        public ICollection<Categoria> GetCategorias()
        {
            return _bd.Categoria.OrderBy(c => c.Nombre).ToList();
        }

        public bool Guardar()
        {
            return _bd.SaveChanges() >= 0 ? true : false;
        }
    }
}
