using APITiendaComida.Models;
using APITiendaComida.Models.DTO;
using APITiendaComida.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APITiendaComida.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductoController : ControllerBase
    {

        private readonly DbapicomidaRapidaContext _context;

        public ProductoController(DbapicomidaRapidaContext context)
        {
            _context = context;
        }

        //listar

        [HttpGet]
        [Route("ObtenerLista")]
        public async Task<IActionResult> ObtenerLista()
        {
            try
            {
                var lista = await _context.Productos.Include(c => c.oCategoria).ToListAsync();
                //var lista = await _context.Productos.ToListAsync();
                return Ok(lista);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("ObtenerPorId/{productoId:int}")]
        public async Task<IActionResult> ObtenerPorId([FromRoute(Name = "productoId")] int id)
        {
            Producto oProductos = _context.Productos.Find(id);
            if (oProductos == null)
            {
                return BadRequest("Producto no encontrado");
            }

            try
            {
                //var item = await _context.Productos.FindAsync(id);
                oProductos = _context.Productos.Include(c => c.oCategoria).Where(p => p.ProductoId == id).FirstOrDefault();

                return Ok(oProductos);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //------------------------------------------------
        [HttpPost("CrearConImagen")]
        public async Task<IActionResult> CrearConImagen([FromForm] CrearProductoDto productoDto)
        {
            try
            {
                if (productoDto == null)
                {
                    return BadRequest("Producto inválido.");
                }

                //// Asegúrate de que CategoriaId es válido
                //if (productoDto.CategoriaId <= 0)
                //{
                //    return BadRequest("CategoriaId debe ser un valor válido.");
                //}

                //// Validar que la categoría existe
                //var categoriaExistente = await _context.Categorias.FindAsync(productoDto.CategoriaId);
                //if (categoriaExistente == null)
                //{
                //    return BadRequest("El CategoriaId proporcionado no existe.");
                //}

                // Crear el objeto Producto a partir del DTO
                var producto = new Producto
                {
                    Nombre = productoDto.Nombre,
                    Descripción = productoDto.Descripcion,
                    Precio = productoDto.Precio,
                    Stock = productoDto.Stock,
                    // CategoriaId = productoDto.CategoriaId
                };

                // Manejo de la imagen
                if (productoDto.Imagen != null)
                {
                    string nombreArchivo = Guid.NewGuid().ToString() + Path.GetExtension(productoDto.Imagen.FileName);
                    string rutaCarpeta = Path.Combine("wwwroot", "ImagenesProductos");
                    string rutaArchivo = Path.Combine(rutaCarpeta, nombreArchivo);

                    // Crear el directorio si no existe
                    if (!Directory.Exists(rutaCarpeta))
                    {
                        Directory.CreateDirectory(rutaCarpeta);
                    }

                    // Guardar el archivo
                    using (var fileStream = new FileStream(rutaArchivo, FileMode.Create))
                    {
                        await productoDto.Imagen.CopyToAsync(fileStream);
                    }

                    // Guardar la URL pública de la imagen
                    var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.PathBase}";
                    producto.ImagenUrl = Path.Combine(baseUrl, "ImagenesProductos", nombreArchivo).Replace("\\", "/"); // Cambia las barras invertidas a barras diagonales
                    producto.ImagenUrllocal = Path.Combine("wwwroot", "ImagenesProductos", nombreArchivo);
                }
                else
                {
                    // Imagen por defecto si no se sube ninguna
                    producto.ImagenUrl = "https://placehold.com/600x400";
                }

                // Guardar el producto en la base de datos
                await _context.Productos.AddAsync(producto);
                await _context.SaveChangesAsync();

                return Ok(new { mensaje = "Producto creado exitosamente", id = producto.ProductoId });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GuardarImagen")]
        public async Task<IActionResult> GuardarImagen([FromForm] UploadFileApi archivo)
        {
            var ruta = string.Empty;

            if (archivo.Archivo.Length > 0)
            {
                var nombreArchivo = Guid.NewGuid().ToString() + Path.GetExtension(archivo.Archivo.FileName);
                ruta = $"Images/{nombreArchivo}";
                using (var stream = new FileStream(ruta, FileMode.Create))
                {
                    try
                    {
                        await archivo.Archivo.CopyToAsync(stream);
                        // TODO: grabar ruta archivo en BD                    
                    }
                    catch (Exception ex)
                    {
                        return BadRequest("Error al grabar archivo: " + ex.Message);
                    }
                }
            }
            return Ok();
        }

        // ------------------------------




        //[HttpPut("Modificar/{productoId:int}")]
        //public async Task<IActionResult> Modificar([FromForm] ModificarProductoDto productoDto, [FromRoute] int productoId)
        //{
        //    try
        //    {
        //        var productoExistente = await _context.Productos.FindAsync(productoId);

        //        if (productoExistente == null)
        //        {
        //            return NotFound("Producto no encontrado.");
        //        }

        //        // Actualiza solo los campos que no son nulos o vacíos
        //        productoExistente.Descripción = productoDto.Descripcion ?? productoExistente.Descripción;
        //        productoExistente.Nombre = productoDto.Nombre ?? productoExistente.Nombre;
        //        productoExistente.Precio = productoDto.Precio != 0 ? productoDto.Precio : productoExistente.Precio;
        //        productoExistente.Stock = productoDto.Stock != 0 ? productoDto.Stock : productoExistente.Stock;

        //        // Asignar CategoriaId directamente si se proporciona
        //        if (productoDto.CategoriaId > 0)
        //        {
        //            productoExistente.CategoriaId = productoDto.CategoriaId;
        //        }

        //        // Manejo de la imagen
        //        if (productoDto.Imagen != null)
        //        {
        //            // Eliminar la imagen anterior si existía
        //            if (!string.IsNullOrEmpty(productoExistente.ImagenUrllocal))
        //            {
        //                var rutaImagenAnterior = Path.Combine(Directory.GetCurrentDirectory(), productoExistente.ImagenUrllocal);
        //                if (System.IO.File.Exists(rutaImagenAnterior))
        //                {
        //                    System.IO.File.Delete(rutaImagenAnterior);
        //                }
        //            }

        //            // Guardar la nueva imagen
        //            string nombreArchivo = Guid.NewGuid().ToString() + Path.GetExtension(productoDto.Imagen.FileName);
        //            string rutaCarpeta = Path.Combine("wwwroot", "ImagenesProductos");
        //            string rutaArchivo = Path.Combine(rutaCarpeta, nombreArchivo);

        //            // Crear el directorio si no existe
        //            if (!Directory.Exists(rutaCarpeta))
        //            {
        //                Directory.CreateDirectory(rutaCarpeta);
        //            }

        //            using (var fileStream = new FileStream(rutaArchivo, FileMode.Create))
        //            {
        //                await productoDto.Imagen.CopyToAsync(fileStream);
        //            }

        //            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.PathBase}";
        //            // Asignar nuevas rutas de imagen
        //            productoExistente.ImagenUrl = Path.Combine(baseUrl, "ImagenesProductos", nombreArchivo);
        //            productoExistente.ImagenUrllocal = Path.Combine("wwwroot", "ImagenesProductos", nombreArchivo);
        //        }

        //        // Actualizar el producto en la base de datos
        //        _context.Productos.Update(productoExistente);
        //        await _context.SaveChangesAsync();

        //        return Ok(new { mensaje = "Producto modificado exitosamente" });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        [HttpPut("Modificar/{productoId:int}")]
        public async Task<IActionResult> Modificar([FromForm] ModificarProductoDto productoDto, [FromRoute] int productoId)
        {
            try
            {
                var productoExistente = await _context.Productos.FindAsync(productoId);

                if (productoExistente == null)
                {
                    return NotFound("Producto no encontrado.");
                }

                // Actualiza solo los campos que no son nulos
                if (!string.IsNullOrEmpty(productoDto.Nombre))
                {
                    productoExistente.Nombre = productoDto.Nombre;
                }

                if (!string.IsNullOrEmpty(productoDto.Descripcion))
                {
                    productoExistente.Descripción = productoDto.Descripcion;
                }

                if (productoDto.Precio.HasValue) // Verifica si hay un nuevo precio
                {
                    productoExistente.Precio = productoDto.Precio.Value;
                }

                if (productoDto.Stock.HasValue) // Verifica si hay un nuevo stock
                {
                    productoExistente.Stock = productoDto.Stock.Value;
                }

                //if (productoDto.CategoriaId.HasValue) // Verifica si hay un nuevo CategoriaId
                //{
                //    productoExistente.CategoriaId = productoDto.CategoriaId.Value;
                //}

                // Manejo de la imagen
                if (productoDto.Imagen != null)
                {
                    // Eliminar la imagen anterior si existía
                    if (!string.IsNullOrEmpty(productoExistente.ImagenUrllocal))
                    {
                        var rutaImagenAnterior = Path.Combine(Directory.GetCurrentDirectory(), productoExistente.ImagenUrllocal);
                        if (System.IO.File.Exists(rutaImagenAnterior))
                        {
                            System.IO.File.Delete(rutaImagenAnterior);
                        }
                    }

                    // Guardar la nueva imagen
                    string nombreArchivo = Guid.NewGuid().ToString() + Path.GetExtension(productoDto.Imagen.FileName);
                    string rutaCarpeta = Path.Combine("wwwroot", "ImagenesProductos");
                    string rutaArchivo = Path.Combine(rutaCarpeta, nombreArchivo);

                    // Crear el directorio si no existe
                    if (!Directory.Exists(rutaCarpeta))
                    {
                        Directory.CreateDirectory(rutaCarpeta);
                    }

                    using (var fileStream = new FileStream(rutaArchivo, FileMode.Create))
                    {
                        await productoDto.Imagen.CopyToAsync(fileStream);
                    }

                    var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.PathBase}";
                    // Asignar nuevas rutas de imagen
                    productoExistente.ImagenUrl = Path.Combine(baseUrl, "ImagenesProductos", nombreArchivo);
                    productoExistente.ImagenUrllocal = Path.Combine("wwwroot", "ImagenesProductos", nombreArchivo);
                }

                // Actualizar el producto en la base de datos
                _context.Productos.Update(productoExistente);
                await _context.SaveChangesAsync();

                return Ok(new { mensaje = "Producto modificado exitosamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpDelete]
        [Route("Eliminar/{productoId:int}")]
        public IActionResult Eliminar(int productoId)
        {
            var oProducto = _context.Productos.Find(productoId);

            if (oProducto == null)
            {
                return BadRequest("Producto no encontrado");
            }

            try
            {
                _context.Productos.Remove(oProducto);
                _context.SaveChanges();

                return StatusCode(StatusCodes.Status200OK, new { mensaje = "ok" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status200OK, new { mensaje = ex.Message });
            }
        }


    }
}
