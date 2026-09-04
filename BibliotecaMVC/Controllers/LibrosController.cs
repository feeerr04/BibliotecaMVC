using BibliotecaMVC.Models;
using BibliotecaMVC.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaMVC.Controllers
{
    public class LibrosController : Controller
    {
        private readonly IRepositorioLibro _repositorioLibro;

        public LibrosController(IRepositorioLibro repositorioLibro)
        {
            _repositorioLibro = repositorioLibro;
        }

        public IActionResult Index()
        {
            var libros = _repositorioLibro.ObtenerTodos();
            return View(libros);
        }

        public IActionResult Details(int id)
        {
            var libro = _repositorioLibro.ObtenerPorId(id);

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

            _repositorioLibro.Agregar(libro);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var libro = _repositorioLibro.ObtenerPorId(id);

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

            _repositorioLibro.Actualizar(libroEditado);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var libro = _repositorioLibro.ObtenerPorId(id);

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
            _repositorioLibro.Eliminar(id);

            return RedirectToAction(nameof(Index));
        }
    }
}
