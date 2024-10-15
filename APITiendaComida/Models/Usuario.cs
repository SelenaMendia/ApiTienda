using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace APITiendaComida.Models;

public partial class Usuario
{
    public int UsuarioId { get; set; }

    public string Nombre { get; set; } = null!;

    public string Usuario1 { get; set; } = null!;

    public string Telefono { get; set; } = null!;

    public string Correo { get; set; } = null!;

    public string Contraseña { get; set; } = null!;

    public string Rol { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<Carrito> Carritos { get; set; } = new List<Carrito>();
}
