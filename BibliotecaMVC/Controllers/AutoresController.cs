using BibliotecaMVC.Models;
using BibliotecaMVC.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaMVC.Controllers
{
    public class AutoresController : Controller
    {
        private readonly BibliotecaContext _context;

        public AutoresController(BibliotecaContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var autores = await _context.Autores.ToListAsync();
            return View(autores);
        }

        public async Task<IActionResult> Details(int id)
        {
            var autor = await _context.Autores.FindAsync(id);

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
        public async Task<IActionResult> Create(Autor autor)
        {
            if (!ModelState.IsValid)
            {
                return View(autor);
            }

            _context.Autores.Add(autor);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Autor agregado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var autor = await _context.Autores.FindAsync(id);

            if (autor == null)
            {
                return NotFound();
            }

            return View(autor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Autor autorEditado)
        {
            if (id != autorEditado.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(autorEditado);
            }

            _context.Autores.Update(autorEditado);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Autor actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var autor = await _context.Autores.FindAsync(id);

            if (autor != null)
            {
                _context.Autores.Remove(autor);
                await _context.SaveChangesAsync();
            }

            TempData["SuccessMessage"] = "Autor eliminado correctamente.";
            return RedirectToAction(nameof(Index));
        }
    }
}
