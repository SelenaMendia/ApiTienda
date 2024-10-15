using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace APITiendaComida.Models;

public partial class Producto
{
    public int ProductoId { get; set; }

    public string Nombre { get; set; } = null!;

    public string Descripción { get; set; } = null!;

    public decimal Precio { get; set; }

    public int Stock { get; set; }

    public string? ImagenUrl { get; set; }

    public string? ImagenUrllocal { get; set; }
    
    [JsonIgnore]
    public int? CategoriaId { get; set; }

    [JsonIgnore]
    public virtual Categoria? oCategoria { get; set; }

    [JsonIgnore]
    public virtual ICollection<DetalleCarrito> DetalleCarritos { get; set; } = new List<DetalleCarrito>();
}
