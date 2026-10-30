using BibliotecaMVC.Models;
using BibliotecaMVC.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BibliotecaMVC.Controllers
{
    [Authorize]
    public class AutoresController : Controller
    {
        private readonly BibliotecaContext _context;
        private readonly ILogger<AutoresController> _logger;

        public AutoresController(BibliotecaContext context, ILogger<AutoresController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Consultando lista de autores.");

            try
            {
                var autores = await _context.Autores.ToListAsync();
                _logger.LogInformation("Se obtuvieron {Cantidad} autores.", autores.Count);
                return View(autores);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Se produjo un error al consultar el listado de autores");
                throw;
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            _logger.LogInformation("Consultando los detalles del autor con identificador {AutorId}", id);

            try
            {
                var autor = await _context.Autores.FindAsync(id);

                if (autor == null)
                {
                    _logger.LogWarning("No se encontró el autor solicitado con identificador {AutorId}", id);
                    return NotFound();
                }

                return View(autor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Se produjo un error al consultar los detalles del autor con identificador {AutorId}", id);
                throw;
            }
        }

        [Authorize(Roles = "Admin, Bibliotecario")]
        public IActionResult Create()
        {
            _logger.LogInformation("Accediendo a la vista de creación de un nuevo autor.");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Bibliotecario")]
        public async Task<IActionResult> Create(Autor autor)
        {
            _logger.LogInformation("El usuario {Usuario} intenta crear un nuevo autor con nombre {AutorNombre}", User.Identity?.Name, autor.Nombre);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("El modelo de autor no es válido al crear. Errores: {ModelStateErrors}",
                    string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                return View(autor);
            }

            try
            {
                _context.Autores.Add(autor);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Autor creado correctamente con identificador {AutorId}", autor.Id);
                TempData["SuccessMessage"] = "Autor agregado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Se produjo un error al guardar el nuevo autor {AutorNombre}", autor.Nombre);
                throw;
            }
        }

        [Authorize(Roles = "Admin, Bibliotecario")]
        public async Task<IActionResult> Edit(int id)
        {
            _logger.LogInformation("Accediendo a la vista de edición del autor con identificador {AutorId}", id);

            try
            {
                var autor = await _context.Autores.FindAsync(id);

                if (autor == null)
                {
                    _logger.LogWarning("No se encontró el autor a editar con identificador {AutorId}", id);
                    return NotFound();
                }

                return View(autor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Se produjo un error al cargar el autor {AutorId} para su edición", id);
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Bibliotecario")]
        public async Task<IActionResult> Edit(int id, Autor autorEditado)
        {
            _logger.LogInformation("El usuario {Usuario} intenta editar el autor con identificador {AutorId}", User.Identity?.Name, id);

            if (id != autorEditado.Id)
            {
                _logger.LogWarning("El identificador de la ruta ({RutaId}) no coincide con el del autor enviado ({AutorId})", id, autorEditado.Id);
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("El modelo de autor no es válido al editar el autor {AutorId}. Errores: {ModelStateErrors}",
                    id, string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                return View(autorEditado);
            }

            try
            {
                _context.Autores.Update(autorEditado);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Autor {AutorId} actualizado correctamente.", id);
                TempData["SuccessMessage"] = "Autor actualizado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                // El registro pudo ser eliminado o modificado por otro usuario mientras se editaba.
                _logger.LogWarning(ex, "No se pudo actualizar el autor {AutorId}: el registro ya no existe o fue modificado.", id);
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Se produjo un error al actualizar el autor {AutorId}", id);
                throw;
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            _logger.LogInformation("El usuario {Usuario} intenta eliminar el autor con identificador {AutorId}", User.Identity?.Name, id);

            try
            {
                var autor = await _context.Autores.FindAsync(id);

                if (autor == null)
                {
                    _logger.LogWarning("No se encontró el autor a eliminar con identificador {AutorId}", id);
                    return NotFound();
                }

                _context.Autores.Remove(autor);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Autor {AutorId} eliminado correctamente.", id);
                TempData["SuccessMessage"] = "Autor eliminado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Se produjo un error al eliminar el autor {AutorId}", id);
                throw;
            }
        }
    }
}
