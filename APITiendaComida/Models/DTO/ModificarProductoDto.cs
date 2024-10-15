using System.Text.Json.Serialization;

namespace APITiendaComida.Models.DTO
{
    public class ModificarProductoDto
    {
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }
        public decimal? Precio { get; set; }
        public int? Stock { get; set; }

        public string? ImagenUrl { get; set; }
        public string? ImagenUrllocal { get; set; }
        public IFormFile? Imagen { get; set; }
       
    }
}
