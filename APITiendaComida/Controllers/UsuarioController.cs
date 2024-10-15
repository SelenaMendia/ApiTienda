using APITiendaComida.Models.DTO;
using APITiendaComida.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APITiendaComida.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly DbapicomidaRapidaContext _context;

        public UsuarioController(DbapicomidaRapidaContext context)
        {
            _context = context;
        }

        // Listar usuarios
        [HttpGet]
        [Route("ObtenerLista")]
        public async Task<IActionResult> ObtenerLista()
        {
            try
            {
                var lista = await _context.Usuarios.ToListAsync();
                return Ok(lista);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Obtener usuario por ID
        [HttpGet("ObtenerPorId/{usuarioId:int}")]
        public async Task<IActionResult> ObtenerPorId([FromRoute(Name = "usuarioId")] int id)
        {
            try
            {
                var usuario = await _context.Usuarios.FindAsync(id);
                if (usuario == null)
                {
                    return BadRequest("Usuario no encontrado");
                }

                return Ok(usuario);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Crear nuevo usuario
        [HttpPost("Crear")]
        public async Task<IActionResult> Crear([FromBody] UsuarioCrearDto usuarioDto)
        {
            try
            {
                if (usuarioDto == null)
                {
                    return BadRequest("Usuario inválido.");
                }

                // Crear el objeto Usuario a partir del DTO
                var usuario = new Usuario
                {
                    Nombre = usuarioDto.Nombre,
                    Usuario1 = usuarioDto.Usuario,
                    Correo = usuarioDto.Correo,
                    Contraseña = usuarioDto.Contraseña,
                    Rol = usuarioDto.Rol,
                    Telefono = usuarioDto.Telefono 
                };

                // Agregar el usuario a la base de datos
                await _context.Usuarios.AddAsync(usuario);
                await _context.SaveChangesAsync();

                return Ok(new { mensaje = "Usuario creado exitosamente", id = usuario.UsuarioId });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //// Modificar usuario
        //[HttpPut("Modificar/{usuarioId:int}")]
        //public async Task<IActionResult> Modificar([FromBody] UsuarioCrearDto usuarioDto, [FromRoute] int usuarioId)
        //{
        //    try
        //    {
        //        var usuarioExistente = await _context.Usuarios.FindAsync(usuarioId);

        //        if (usuarioExistente == null)
        //        {
        //            return NotFound("Usuario no encontrado.");
        //        }

        //        // Actualiza solo los campos que no son nulos o vacíos
        //        usuarioExistente.Nombre = usuarioDto.Nombre ?? usuarioExistente.Nombre;
        //        usuarioExistente.Usuario1 = usuarioDto.Usuario ?? usuarioExistente.Usuario1;
        //        usuarioExistente.Correo = usuarioDto.Correo ?? usuarioExistente.Correo;
        //        usuarioExistente.Contraseña = usuarioDto.Contraseña ?? usuarioExistente.Contraseña;
        //        usuarioExistente.Rol = usuarioDto.Rol ?? usuarioExistente.Rol;
        //        usuarioExistente.Telefono = usuarioDto.Telefono ?? usuarioExistente.Telefono; 

        //        _context.Usuarios.Update(usuarioExistente);
        //        await _context.SaveChangesAsync();

        //        return Ok(new { mensaje = "Usuario modificado exitosamente" });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}
        [HttpPut("Modificar/{usuarioId:int}")]
        public async Task<IActionResult> Modificar([FromForm] UsuarioModificarDto usuarioDto, [FromRoute] int usuarioId) //importante el [FromForm]
        {
            try
            {
                var usuarioExistente = await _context.Usuarios.FindAsync(usuarioId);
                if (usuarioExistente == null)
                {
                    return NotFound("Usuario no encontrado.");
                }

                // Actualiza solo los campos que son proporcionados
                if (!string.IsNullOrEmpty(usuarioDto.Nombre))
                {
                    usuarioExistente.Nombre = usuarioDto.Nombre;
                }

                if (!string.IsNullOrEmpty(usuarioDto.Usuario))
                {
                    usuarioExistente.Usuario1 = usuarioDto.Usuario;
                }

                if (!string.IsNullOrEmpty(usuarioDto.Telefono))
                {
                    usuarioExistente.Telefono = usuarioDto.Telefono;
                }

                if (!string.IsNullOrEmpty(usuarioDto.Correo))
                {
                    usuarioExistente.Correo = usuarioDto.Correo;
                }

                if (!string.IsNullOrEmpty(usuarioDto.Contraseña))
                {
                    usuarioExistente.Contraseña = usuarioDto.Contraseña; // Asegúrate de encriptar la contraseña aquí
                }

                if (!string.IsNullOrEmpty(usuarioDto.Rol))
                {
                    usuarioExistente.Rol = usuarioDto.Rol;
                }

                // Guarda los cambios
                await _context.SaveChangesAsync();

                return Ok(new { mensaje = "Usuario modificado exitosamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }







        // Eliminar usuario
        [HttpDelete]
        [Route("Eliminar/{usuarioId:int}")]
        public IActionResult Eliminar(int usuarioId)
        {
            var oUsuario = _context.Usuarios.Find(usuarioId);

            if (oUsuario == null)
            {
                return BadRequest("Usuario no encontrado");
            }

            try
            {
                _context.Usuarios.Remove(oUsuario);
                _context.SaveChanges();

                return StatusCode(StatusCodes.Status200OK, new { mensaje = "Usuario eliminado exitosamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status200OK, new { mensaje = ex.Message });
            }
        }

        //login 
        [HttpPost("ValidarCredencial")]
        public async Task<IActionResult> ValidarCredencial([FromBody] UsuarioLoginDto usuario)
        {
            var usuarioLogin = await _context.Usuarios
                .FirstOrDefaultAsync(x =>
                    (x.Correo.Equals(usuario.Email) || x.Usuario1.Equals(usuario.Email)) // Validación por Correo o Usuario1
                    && x.Contraseña.Equals(usuario.Password));

            if (usuarioLogin == null)
            {
                return NotFound("Usuario no encontrado");
            }

            LoginResponseDto loginResponse = new LoginResponseDto()
            {
                Correo = usuarioLogin.Correo,
                Nombre = usuarioLogin.Nombre,
                Rol = usuarioLogin.Rol,
                Telefono = usuarioLogin.Telefono,
                UsuarioId = usuarioLogin.UsuarioId
            };

            return Ok(loginResponse);
        }

    }

}
