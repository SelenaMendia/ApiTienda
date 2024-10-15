using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace APITiendaComida.Models.DTO
{
    public class CrearProductoDto
    {
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public IFormFile? Imagen { get; set; }

        //public FileResult? Imagen { get; set; }


    }
}
