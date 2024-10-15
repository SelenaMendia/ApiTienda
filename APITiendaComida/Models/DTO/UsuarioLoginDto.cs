namespace APITiendaComida.Models.DTO
{
    public class UsuarioLoginDto
    {
        public string Email { get; set; } = null!;  // Puede ser Correo o Usuario1
        public string Password { get; set; } = null!;
    }
}
