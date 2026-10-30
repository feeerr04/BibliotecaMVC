using Microsoft.AspNetCore.Mvc;

namespace BibliotecaMVC.Components
{
    // View Component que muestra los accesos de navegación disponibles
    // según el estado de sesión y el rol del usuario actual.
    public class AccesosRapidosViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var accesos = new List<(string Texto, string Controlador, string Accion)>();

            if (User.Identity?.IsAuthenticated == true)
            {
                accesos.Add(("Libros", "Libros", "Index"));
                accesos.Add(("Autores", "Autores", "Index"));
                accesos.Add(("Categorías", "Categorias", "Index"));

                if (UserClaimsPrincipal.IsInRole("Admin"))
                {
                    accesos.Add(("Administrar usuarios", "Usuarios", "Index"));
                }
            }
            else
            {
                accesos.Add(("Iniciar sesión", "Account", "Login"));
            }

            return View(accesos);
        }
    }
}
