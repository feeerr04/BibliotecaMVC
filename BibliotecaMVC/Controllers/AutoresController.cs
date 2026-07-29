using BibliotecaMVC.Models;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaMVC.Controllers
{
    public class AutoresController : Controller
    {
        private static List<Autor> _autores = new List<Autor>
        {
            new Autor { Id = 1, Nombre = "Gabriel García Márquez", Nacionalidad = "Colombiana", FechaNacimiento = new DateTime(1927, 3, 6) },
            new Autor { Id = 2, Nombre = "Isabel Allende", Nacionalidad = "Chilena", FechaNacimiento = new DateTime(1942, 8, 2) },
            new Autor { Id = 3, Nombre = "Mario Vargas Llosa", Nacionalidad = "Peruana", FechaNacimiento = new DateTime(1936, 3, 28) }
        };

        public IActionResult Index()
        {
            return View(_autores);
        }

        public IActionResult Details(int id)
        {
            var autor = _autores.FirstOrDefault(a => a.Id == id);

            if (autor == null)
            {
                return NotFound();
            }

            return View(autor);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Autor autor)
        {
            if (!ModelState.IsValid)
            {
                return View(autor);
            }

            if (_autores.Any())
            {
                autor.Id = _autores.Max(x => x.Id) + 1;
            }
            else
            {
                autor.Id = 1;
            }

            _autores.Add(autor);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var autor = _autores.FirstOrDefault(a => a.Id == id);

            if (autor == null)
            {
                return NotFound();
            }

            return View(autor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Autor autorEditado)
        {
            if (id != autorEditado.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(autorEditado);
            }

            var autor = _autores.FirstOrDefault(a => a.Id == id);

            if (autor == null)
            {
                return NotFound();
            }

            autor.Nombre = autorEditado.Nombre;
            autor.Nacionalidad = autorEditado.Nacionalidad;
            autor.FechaNacimiento = autorEditado.FechaNacimiento;

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var autor = _autores.FirstOrDefault(a => a.Id == id);

            if (autor == null)
            {
                return NotFound();
            }

            return View(autor);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var autor = _autores.FirstOrDefault(a => a.Id == id);

            if (autor == null)
            {
                return NotFound();
            }

            _autores.Remove(autor);

            return RedirectToAction(nameof(Index));
        }
    }
}
