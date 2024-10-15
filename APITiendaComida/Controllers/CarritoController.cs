namespace APITiendaComida.Controllers
{
    using APITiendaComida.Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using APITiendaComida.Models;

    namespace Tp6API.Controllers
    {
        [Route("api/[controller]")]
        [ApiController]
        public class CarritoController : ControllerBase
        {
            private readonly DbapicomidaRapidaContext _context;

            public CarritoController(DbapicomidaRapidaContext context)
            {
                _context = context;
            }

            // Obtener lista de carritos
            [HttpGet]
            [Route("ObtenerLista")]
            public async Task<IActionResult> ObtenerLista()
            {
                try
                {
                    var lista = await _context.Carritos.Include(c => c.DetalleCarritos).ToListAsync();
                    return Ok(lista);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

            // Obtener carrito por ID
            [HttpGet("ObtenerPorId/{id:int}")]
            public async Task<IActionResult> ObtenerPorId([FromRoute] int id)
            {
                try
                {
                    var carrito = await _context.Carritos
                        .Include(c => c.DetalleCarritos)
                        .ThenInclude(dc => dc.Producto) // Incluye los productos del carrito
                        .FirstOrDefaultAsync(c => c.CarritoId == id);

                    if (carrito == null)
                    {
                        return NotFound("Carrito no encontrado.");
                    }

                    return Ok(carrito);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

            // Crear un nuevo carrito
            [HttpPost("Crear")]
            public async Task<IActionResult> Crear([FromBody] Carrito carrito)
            {
                try
                {
                    if (carrito == null)
                    {
                        return BadRequest("Datos inválidos.");
                    }

                    await _context.Carritos.AddAsync(carrito);
                    await _context.SaveChangesAsync();

                    return Ok(new { mensaje = "Carrito creado exitosamente", id = carrito.CarritoId });
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

            // Modificar un carrito existente
            [HttpPut("Modificar/{id:int}")]
            public async Task<IActionResult> Modificar([FromBody] Carrito carritoActualizado, [FromRoute] int id)
            {
                try
                {
                    var carritoExistente = await _context.Carritos.FindAsync(id);
                    if (carritoExistente == null)
                    {
                        return NotFound("Carrito no encontrado.");
                    }

                    carritoExistente.FechaCreacion = carritoActualizado.FechaCreacion ?? carritoExistente.FechaCreacion;
                    carritoExistente.PrecioTotal = carritoActualizado.PrecioTotal != 0 ? carritoActualizado.PrecioTotal : carritoExistente.PrecioTotal;

                    _context.Carritos.Update(carritoExistente);
                    await _context.SaveChangesAsync();

                    return Ok(new { mensaje = "Carrito modificado exitosamente" });
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

            // Eliminar un carrito por ID
            [HttpDelete("Eliminar/{id:int}")]
            public async Task<IActionResult> Eliminar([FromRoute] int id)
            {
                try
                {
                    var carrito = await _context.Carritos.FindAsync(id);
                    if (carrito == null)
                    {
                        return NotFound("Carrito no encontrado.");
                    }

                    _context.Carritos.Remove(carrito);
                    await _context.SaveChangesAsync();

                    return Ok(new { mensaje = "Carrito eliminado exitosamente" });
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

        }
    }

}
