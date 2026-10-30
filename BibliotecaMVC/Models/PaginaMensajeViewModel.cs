namespace BibliotecaMVC.Models
{
    // Datos para la vista parcial reutilizable _PaginaMensaje
    // (usada por las páginas de error y de acceso denegado).
    public class PaginaMensajeViewModel
    {
        // "error", "lock" o "search"
        public string Icono { get; set; } = "error";

        // "danger" o "warning"
        public string Color { get; set; } = "danger";

        public string? Codigo { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;
        public string? Detalle { get; set; }
        public string? Referencia { get; set; }
    }
}
