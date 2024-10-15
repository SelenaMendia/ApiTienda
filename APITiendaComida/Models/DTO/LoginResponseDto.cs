namespace APITiendaComida.Models.DTO
{
    public class LoginResponseDto
    {
        public string Correo { get; set; } = null!;   // Correo en lugar de Email
        public string Nombre { get; set; } = null!;
        public string Rol { get; set; } = null!;
        public string Telefono { get; set; } = null!;  // Añadir Telefono
        public int UsuarioId { get; set; }
    }
}
