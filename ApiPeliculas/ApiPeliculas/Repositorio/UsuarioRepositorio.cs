using ApiPeliculas.Data;
using ApiPeliculas.Modelos;
using ApiPeliculas.Modelos.Dtos;
using ApiPeliculas.Repositorio.IRepositorio;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ApiPeliculas.Repositorio
{
    public class UsuarioRepositorio : IUsuarioRepositorio
    {
        private readonly AplicationDbContext _bd;
        private string claveSecreta;

        public UsuarioRepositorio(AplicationDbContext bd, IConfiguration config)
        {
            _bd= bd;
            claveSecreta = config.GetValue<string>("ApiSettings:Secreta");
        }
        public Usuario GetUsuario(int usuarioId)
        {
            return _bd.Usuario.FirstOrDefault(u => u.Id == usuarioId);
        }

        public ICollection<Usuario> GetUsuarios()
        {
            return _bd.Usuario.OrderBy(u => u.NombreUsuario).ToList();
        }

        public bool IsUniqueUser(string usuario)
        {
            var usuariobd = _bd.Usuario.FirstOrDefault(u => u.NombreUsuario == usuario);
            if (usuariobd == null)
            {
                return true;
            }
            return false;
        }

        public async Task<UsuarioLoginRespuestaDTO> Login(UsuarioLoginDTO usuarioLoginDto)
        {
            var passwordEncriptado = obtenermd5(usuarioLoginDto.Password);

            // Utilizar FirstOrDefaultAsync para operaciones asincrónicas en la base de datos
            var usuario = await _bd.Usuario.FirstOrDefaultAsync(
                u => u.NombreUsuario.ToLower() == usuarioLoginDto.NombreUsuario.ToLower()
                && u.Password == passwordEncriptado
            );

            if (usuario == null)
            {
                return new UsuarioLoginRespuestaDTO
                {
                    Token = "",
                    Usuario = null
                };
            }
            var manejarToken = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(claveSecreta);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, usuario.NombreUsuario.ToString() ),
                    new Claim(ClaimTypes.Role, usuario.Rol)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = manejarToken.CreateToken(tokenDescriptor);

            UsuarioLoginRespuestaDTO usuarioLoginRespuestaDto = new UsuarioLoginRespuestaDTO
            {
                Token = manejarToken.WriteToken(token),
                Usuario = usuario
            };
            return usuarioLoginRespuestaDto;
        }

        public async Task<Usuario> Registro(UsuarioRegistroDTO usuarioRegistroDto)
        {
            var passwordEncriptado = obtenermd5(usuarioRegistroDto.Password);
            Usuario usuario = new Usuario()
            {
                NombreUsuario = usuarioRegistroDto.NombreUsuario,
                Password = passwordEncriptado,
                Nombre = usuarioRegistroDto.Nombre,
                Rol = usuarioRegistroDto.Rol
            };
            _bd.Usuario.Add(usuario);

            await _bd.SaveChangesAsync();
            usuario.Password = passwordEncriptado;
            return usuario;
        }

        public static string obtenermd5(string valor)
        {
            MD5CryptoServiceProvider x = new MD5CryptoServiceProvider();
            byte[] data = System.Text.Encoding.UTF8.GetBytes(valor);
            data = x.ComputeHash(data);
            string resp = "";
            for (int i = 0; i < data.Length; i++)
                resp += data[i].ToString("x2").ToLower();
            return resp;

        }
    }
}

