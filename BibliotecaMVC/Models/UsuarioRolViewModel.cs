namespace BibliotecaMVC.Models
{
    // Datos que necesita la vista de administración de usuarios.
    public class UsuarioRolViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string RolActual { get; set; } = "Sin rol";
        public bool EsUsuarioActual { get; set; }
    }
}
