using BibliotecaMVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace BibliotecaMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Usuarios()
        {
            return View();
        }

        public IActionResult Prestamos()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult About()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [AllowAnonymous]
        public IActionResult Error(int? codigo = null)
        {
            var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            _logger.LogWarning("Se mostró la página de error. Código: {Codigo}. Referencia: {RequestId}", codigo ?? 500, requestId);

            return View(new ErrorViewModel { RequestId = requestId, StatusCode = codigo });
        }

        // Acción de prueba (solo Admin) para comprobar la página de error personalizada
        // y el registro de excepciones. Puede eliminarse después de las pruebas.
        [Authorize(Roles = "Admin")]
        public IActionResult ProbarError()
        {
            throw new InvalidOperationException("Excepción de prueba generada desde /Home/ProbarError.");
        }
    }
}
