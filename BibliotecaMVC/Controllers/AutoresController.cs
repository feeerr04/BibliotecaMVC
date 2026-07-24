using BibliotecaMVC.Models;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaMVC.Controllers
{
    public class AutoresController : Controller
    {
        public IActionResult Index()
        {
            List<Autor> autores = new List<Autor>()
            {
                new Autor
                {
                    Id = 1,
                    Nombre = "Robert",
                    Apellido = "Martin",
                    Nacionalidad = "Estados Unidos",
                    FechaNacimiento = new DateTime(1952, 12, 5),
                    Activo = true
                },
                new Autor
                {
                    Id = 2,
                    Nombre = "Andrew",
                    Apellido = "Hunt",
                    Nacionalidad = "Estados Unidos",
                    FechaNacimiento = new DateTime(1964, 3, 5),
                    Activo = true
                },
                new Autor
                {
                    Id = 3,
                    Nombre = "Martin",
                    Apellido = "Fowler",
                    Nacionalidad = "Reino Unido",
                    FechaNacimiento = new DateTime(1963, 12, 18),
                    Activo = false
                },
                new Autor
                {
                    Id = 4,
                    Nombre = "Kent",
                    Apellido = "Beck",
                    Nacionalidad = "Estados Unidos",
                    FechaNacimiento = new DateTime(1961, 3, 31),
                    Activo = true
                },
                new Autor
                {
                    Id = 5,
                    Nombre = "Erich",
                    Apellido = "Gamma",
                    Nacionalidad = "Suiza",
                    FechaNacimiento = new DateTime(1961, 3, 13),
                    Activo = false
                }
            };

            ViewBag.Autores = autores;
            return View();
        }
    }
}
