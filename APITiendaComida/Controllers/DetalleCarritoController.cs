using APITiendaComida.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APITiendaComida.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DetalleCarritoController : ControllerBase
    {
        private readonly DbapicomidaRapidaContext _context;

        public DetalleCarritoController(DbapicomidaRapidaContext context)
        {
            _context = context;
        }

        // Obtener lista de detalles de carritos
        [HttpGet]
        [Route("ObtenerLista")]
        public async Task<IActionResult> ObtenerLista()
        {
            try
            {
                var lista = await _context.DetalleCarritos.Include(dc => dc.Producto).ToListAsync();
                return Ok(lista);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Obtener detalle de carrito por ID
        [HttpGet("ObtenerPorId/{id:int}")]
        public async Task<IActionResult> ObtenerPorId([FromRoute] int id)
        {
            try
            {
                var detalle = await _context.DetalleCarritos
                    .Include(dc => dc.Producto)
                    .FirstOrDefaultAsync(dc => dc.DetalleId == id);

                if (detalle == null)
                {
                    return NotFound("Detalle de carrito no encontrado.");
                }

                return Ok(detalle);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Obtener detalles de carrito por CarritoId
        [HttpGet("ObtenerPorIdCarrito/{carritoId:int}")]
        public async Task<IActionResult> ObtenerPorCarritoId([FromRoute] int carritoId)
        {
            try
            {
                var detalles = await _context.DetalleCarritos
                    .Include(dc => dc.Producto)
                    .Where(dc => dc.CarritoId == carritoId)
                    .ToListAsync();

                if (!detalles.Any())
                {
                    return NotFound("No se encontraron detalles para este carrito.");
                }

                return Ok(detalles);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // Crear un nuevo detalle de carrito
        [HttpPost("Crear")]
        public async Task<IActionResult> Crear([FromBody] DetalleCarrito detalleCarrito)
        {
            try
            {
                if (detalleCarrito == null)
                {
                    return BadRequest("Datos inválidos.");
                }

                await _context.DetalleCarritos.AddAsync(detalleCarrito);
                await _context.SaveChangesAsync();

                return Ok(new { mensaje = "Detalle de carrito creado exitosamente", id = detalleCarrito.DetalleId });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Modificar un detalle de carrito existente
        [HttpPut("Modificar/{id:int}")]
        public async Task<IActionResult> Modificar([FromBody] DetalleCarrito detalleActualizado, [FromRoute] int id)
        {
            try
            {
                var detalleExistente = await _context.DetalleCarritos.FindAsync(id);
                if (detalleExistente == null)
                {
                    return NotFound("Detalle de carrito no encontrado.");
                }

                detalleExistente.Cantidad = detalleActualizado.Cantidad != 0 ? detalleActualizado.Cantidad : detalleExistente.Cantidad;
                detalleExistente.PrecioUnitario = detalleActualizado.PrecioUnitario != 0 ? detalleActualizado.PrecioUnitario : detalleExistente.PrecioUnitario;

                _context.DetalleCarritos.Update(detalleExistente);
                await _context.SaveChangesAsync();

                return Ok(new { mensaje = "Detalle de carrito modificado exitosamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Eliminar un detalle de carrito por ID
        [HttpDelete("Eliminar/{id:int}")]
        public async Task<IActionResult> Eliminar([FromRoute] int id)
        {
            try
            {
                var detalle = await _context.DetalleCarritos.FindAsync(id);
                if (detalle == null)
                {
                    return NotFound("Detalle de carrito no encontrado.");
                }

                _context.DetalleCarritos.Remove(detalle);
                await _context.SaveChangesAsync();

                return Ok(new { mensaje = "Detalle de carrito eliminado exitosamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
