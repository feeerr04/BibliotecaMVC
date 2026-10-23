using BibliotecaMVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaMVC.Controllers
{
    // Solo el Admin puede administrar usuarios y cambiar sus roles.
    [Authorize(Roles = "Admin")]
    public class UsuariosController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsuariosController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var usuarios = await _userManager.Users.OrderBy(u => u.UserName).ToListAsync();
            var idActual = _userManager.GetUserId(User);
            var modelo = new List<UsuarioRolViewModel>();

            foreach (var usuario in usuarios)
            {
                var roles = await _userManager.GetRolesAsync(usuario);
                modelo.Add(new UsuarioRolViewModel
                {
                    Id = usuario.Id,
                    UserName = usuario.UserName ?? string.Empty,
                    Email = usuario.Email,
                    RolActual = roles.FirstOrDefault() ?? "Sin rol",
                    EsUsuarioActual = usuario.Id == idActual
                });
            }

            ViewBag.Roles = await _roleManager.Roles
                .Select(r => r.Name!)
                .OrderBy(n => n)
                .ToListAsync();

            return View(modelo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarRol(string userId, string nuevoRol)
        {
            var usuario = await _userManager.FindByIdAsync(userId);
            if (usuario == null)
            {
                return NotFound();
            }

            // Evita que el Admin se quite su propio rol y se quede sin acceso a esta pantalla.
            if (usuario.Id == _userManager.GetUserId(User))
            {
                TempData["ErrorMessage"] = "No puedes cambiar tu propio rol.";
                return RedirectToAction(nameof(Index));
            }

            // Valida que el rol enviado exista (no confiar en lo que llega del formulario).
            if (string.IsNullOrWhiteSpace(nuevoRol) || !await _roleManager.RoleExistsAsync(nuevoRol))
            {
                TempData["ErrorMessage"] = "El rol seleccionado no es válido.";
                return RedirectToAction(nameof(Index));
            }

            var rolesActuales = await _userManager.GetRolesAsync(usuario);
            if (rolesActuales.Count == 1 && rolesActuales.Contains(nuevoRol))
            {
                TempData["ErrorMessage"] = $"{usuario.UserName} ya tiene el rol {nuevoRol}.";
                return RedirectToAction(nameof(Index));
            }

            // Cada usuario tiene un solo rol: se quitan los actuales y se asigna el nuevo.
            var quitar = await _userManager.RemoveFromRolesAsync(usuario, rolesActuales);
            if (!quitar.Succeeded)
            {
                TempData["ErrorMessage"] = string.Join(", ", quitar.Errors.Select(e => e.Description));
                return RedirectToAction(nameof(Index));
            }

            var agregar = await _userManager.AddToRoleAsync(usuario, nuevoRol);
            if (!agregar.Succeeded)
            {
                TempData["ErrorMessage"] = string.Join(", ", agregar.Errors.Select(e => e.Description));
                return RedirectToAction(nameof(Index));
            }

            TempData["SuccessMessage"] = $"Rol de {usuario.UserName} actualizado a {nuevoRol}.";
            return RedirectToAction(nameof(Index));
        }
    }
}
