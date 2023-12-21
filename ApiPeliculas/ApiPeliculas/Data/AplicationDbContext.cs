using ApiPeliculas.Modelos;
using Microsoft.EntityFrameworkCore;

namespace ApiPeliculas.Data
{
    public class AplicationDbContext : DbContext
    {
        public AplicationDbContext(DbContextOptions<AplicationDbContext> options) : base(options)
        {

        }
        // Agregar los modelos Aqui
        public DbSet<Categoria> Categoria { get; set; } // Coneccion con la tabla categoria de la base de datos
        public DbSet<Pelicula> Pelicula { get; set; } // Coneccion con la tabla categoria de la base de datos
        public DbSet<Usuario> Usuario { get; set; }
    }
}
