using BibliotecaMVC.Models;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaMVC.Controllers
{
    public class LibrosController : Controller
    {
        private static List<Libro> _libros = new List<Libro>
        {
            new Libro { Id = 1, Titulo = "Clean Code", Autor = "Robert C. Martin", Categoria = "Programación", Precio = 35.5m, Disponible = true, ImagenUrl = "clean-code.jpg" },
            new Libro { Id = 2, Titulo = "The Pragmatic Programmer", Autor = "Andrew Hunt", Categoria = "Programación", Precio = 40.0m, Disponible = false, ImagenUrl = "pragmatic.jpg" }
        };

        public IActionResult Index()
        {
            return View(_libros);
        }

        public IActionResult Details(int id)
        {
            var libro = _libros.FirstOrDefault(l => l.Id == id);

            if (libro == null)
            {
                return NotFound();
            }

            return View(libro);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Libro libro)
        {
            if (!ModelState.IsValid)
            {
                return View(libro);
            }

            if (_libros.Any())
            {
                libro.Id = _libros.Max(x => x.Id) + 1;
            }
            else
            {
                libro.Id = 1;
            }

            if (string.IsNullOrEmpty(libro.ImagenUrl))
            {
                libro.ImagenUrl = "sin-imagen.png";
            }

            _libros.Add(libro);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var libro = _libros.FirstOrDefault(l => l.Id == id);

            if (libro == null)
            {
                return NotFound();
            }

            return View(libro);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Libro libroEditado)
        {
            if (id != libroEditado.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(libroEditado);
            }

            var libro = _libros.FirstOrDefault(l => l.Id == id);

            if (libro == null)
            {
                return NotFound();
            }

            libro.Titulo = libroEditado.Titulo;
            libro.Autor = libroEditado.Autor;
            libro.Categoria = libroEditado.Categoria;
            libro.Precio = libroEditado.Precio;
            libro.Disponible = libroEditado.Disponible;

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var libro = _libros.FirstOrDefault(l => l.Id == id);

            if (libro == null)
            {
                return NotFound();
            }

            return View(libro);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var libro = _libros.FirstOrDefault(l => l.Id == id);

            if (libro == null)
            {
                return NotFound();
            }

            _libros.Remove(libro);

            return RedirectToAction(nameof(Index));
        }
    }
}
