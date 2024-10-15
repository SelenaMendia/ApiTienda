using System;
using System.Collections.Generic;

namespace APITiendaComida.Models;

public partial class Carrito
{
    public int CarritoId { get; set; }

    public int? UsuarioId { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public decimal PrecioTotal { get; set; }

    public virtual ICollection<DetalleCarrito> DetalleCarritos { get; set; } = new List<DetalleCarrito>();

    public virtual Usuario? Usuario { get; set; }
}
