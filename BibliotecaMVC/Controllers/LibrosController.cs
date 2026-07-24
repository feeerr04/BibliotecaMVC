using BibliotecaMVC.Models;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaMVC.Controllers
{
    public class LibrosController : Controller
    {
        public IActionResult Index()
        {
            List<Libro> libros = new List<Libro>()
            {
                new Libro
                {
                    ID = 1,
                    Titulo = "Clean Code",
                    Autor = "Robert C. Martin",
                    Categoria = "Programación",
                    Precio = 35.5m,
                    Disponible = true
                },
                new Libro
                {
                    ID = 2,
                    Titulo = "The Pragmatic Programmer",
                    Autor = "Andrew Hunt",
                    Categoria = "Programación",
                    Precio = 40.0m,
                    Disponible = false
                }
            };

            ViewBag.Libros = libros;

            return View();
        }
    }
}
