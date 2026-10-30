using BibliotecaMVC.Models;
using BibliotecaMVC.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace BibliotecaMVC.Controllers
{
    // Consultar (Index y Details): Admin, Bibliotecario y Usuario.
    // Agregar y editar: Admin y Bibliotecario. Eliminar: solo Admin.
    [Authorize(Roles = "Admin, Bibliotecario, Usuario")]
    public class LibrosController : Controller
    {
        private readonly BibliotecaContext _context;
        private readonly ILogger<LibrosController> _logger;

        public LibrosController(BibliotecaContext context, ILogger<LibrosController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Consultando lista de libros.");

            try
            {
                var libros = await _context.Libros.ToListAsync();
                _logger.LogInformation("Se obtuvieron {Cantidad} libros.", libros.Count);
                return View(libros);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Se produjo un error al consultar el listado de libros");
                throw;
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            _logger.LogInformation("Consultando los detalles del libro con identificador {LibroId}", id);

            try
            {
                var libro = await _context.Libros.FindAsync(id);

                if (libro == null)
                {
                    _logger.LogWarning("No se encontró el libro solicitado con identificador {LibroId}", id);
                    return NotFound();
                }

                return View(libro);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Se produjo un error al consultar los detalles del libro con identificador {LibroId}", id);
                throw;
            }
        }

        [Authorize(Roles = "Admin, Bibliotecario")]
        public IActionResult Create()
        {
            _logger.LogInformation("Accediendo a la vista de creación de un nuevo libro.");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Bibliotecario")]
        public async Task<IActionResult> Create(Libro libro)
        {
            _logger.LogInformation("El usuario {Usuario} intenta crear un nuevo libro con título {LibroTitulo}", User.Identity?.Name, libro.Titulo);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("El modelo de libro no es válido al crear. Errores: {ModelStateErrors}",
                    string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                return View(libro);
            }

            try
            {
                _context.Libros.Add(libro);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Libro creado correctamente con identificador {LibroId}", libro.Id);
                TempData["SuccessMessage"] = "Libro agregado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Se produjo un error al guardar el nuevo libro {LibroTitulo}", libro.Titulo);
                throw;
            }
        }

        [Authorize(Roles = "Admin, Bibliotecario")]
        public async Task<IActionResult> Edit(int id)
        {
            _logger.LogInformation("Accediendo a la vista de edición del libro con identificador {LibroId}", id);

            try
            {
                var libro = await _context.Libros.FindAsync(id);

                if (libro == null)
                {
                    _logger.LogWarning("No se encontró el libro a editar con identificador {LibroId}", id);
                    return NotFound();
                }

                return View(libro);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Se produjo un error al cargar el libro {LibroId} para su edición", id);
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Bibliotecario")]
        public async Task<IActionResult> Edit(int id, Libro libroEditado)
        {
            _logger.LogInformation("El usuario {Usuario} intenta editar el libro con identificador {LibroId}", User.Identity?.Name, id);

            if (id != libroEditado.Id)
            {
                _logger.LogWarning("El identificador de la ruta ({RutaId}) no coincide con el del libro enviado ({LibroId})", id, libroEditado.Id);
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("El modelo de libro no es válido al editar el libro {LibroId}. Errores: {ModelStateErrors}",
                    id, string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                return View(libroEditado);
            }

            try
            {
                _context.Libros.Update(libroEditado);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Libro {LibroId} actualizado correctamente.", id);
                TempData["SuccessMessage"] = "Libro actualizado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                // El registro pudo ser eliminado o modificado por otro usuario mientras se editaba.
                _logger.LogWarning(ex, "No se pudo actualizar el libro {LibroId}: el registro ya no existe o fue modificado.", id);
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Se produjo un error al actualizar el libro {LibroId}", id);
                throw;
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            _logger.LogInformation("El usuario {Usuario} intenta eliminar el libro con identificador {LibroId}", User.Identity?.Name, id);

            try
            {
                var libro = await _context.Libros.FindAsync(id);

                if (libro == null)
                {
                    _logger.LogWarning("No se encontró el libro a eliminar con identificador {LibroId}", id);
                    return NotFound();
                }

                _context.Libros.Remove(libro);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Libro {LibroId} eliminado correctamente.", id);
                TempData["SuccessMessage"] = "Libro eliminado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Se produjo un error al eliminar el libro {LibroId}", id);
                throw;
            }
        }
    }
}
