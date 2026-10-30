using Microsoft.AspNetCore.Mvc;
using BibliotecaMVC.Models;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace BibliotecaMVC.Controllers
{
    // Misma matriz de permisos que Libros:
    // consultar (todos los roles), agregar/editar (Admin y Bibliotecario), eliminar (solo Admin).
    [Authorize(Roles = "Admin, Bibliotecario, Usuario")]
    public class CategoriasController : Controller
    {
        private readonly string _connectionString;
        private readonly ILogger<CategoriasController> _logger;

        public CategoriasController(IConfiguration configuration, ILogger<CategoriasController> logger)
        {
            _connectionString = configuration.GetConnectionString("BibliotecaDB");
            _logger = logger;
        }

        public IActionResult Index()
        {
            _logger.LogInformation("Consultando lista de categorías.");

            var categorias = new List<Categoria>();

            try
            {
                using (var conexion = new SqlConnection(_connectionString))
                {
                    var sql = "SELECT ID, Nombre, Descripcion FROM Categorias";
                    using (var comando = new SqlCommand(sql, conexion))
                    {
                        conexion.Open();
                        using (var lector = comando.ExecuteReader())
                        {
                            while (lector.Read())
                            {
                                categorias.Add(new Categoria
                                {
                                    ID = lector.GetInt32(0),
                                    Nombre = lector.GetString(1),
                                    Descripcion = lector.IsDBNull(2) ? null : lector.GetString(2)
                                });
                            }
                        }
                    }
                }

                _logger.LogInformation("Se obtuvieron {Cantidad} categorías.", categorias.Count);
                return View(categorias);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Se produjo un error al consultar el listado de categorías");
                throw;
            }
        }

        [Authorize(Roles = "Admin, Bibliotecario")]
        public IActionResult Create()
        {
            _logger.LogInformation("Accediendo a la vista de creación de una nueva categoría.");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Bibliotecario")]
        public IActionResult Create(Categoria categoria)
        {
            if (categoria == null || string.IsNullOrWhiteSpace(categoria.Nombre))
            {
                _logger.LogWarning("Intento de crear una categoría sin nombre por el usuario {Usuario}", User.Identity?.Name);
                ModelState.AddModelError("Nombre", "El nombre es obligatorio.");
                return View(categoria);
            }

            _logger.LogInformation("El usuario {Usuario} intenta crear una nueva categoría con nombre {CategoriaNombre}", User.Identity?.Name, categoria.Nombre);

            try
            {
                using (var conexion = new SqlConnection(_connectionString))
                {
                    var sql = "INSERT INTO Categorias (Nombre, Descripcion) VALUES (@Nombre, @Descripcion)";

                    using (var comando = new SqlCommand(sql, conexion))
                    {
                        comando.Parameters.AddWithValue("@Nombre", categoria.Nombre);
                        comando.Parameters.AddWithValue("@Descripcion", (object)categoria.Descripcion ?? System.DBNull.Value);

                        conexion.Open();
                        comando.ExecuteNonQuery();
                    }
                }

                _logger.LogInformation("Categoría {CategoriaNombre} creada correctamente.", categoria.Nombre);
                TempData["SuccessMessage"] = "Categoría guardada correctamente.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Se produjo un error al guardar la categoría {CategoriaNombre}", categoria.Nombre);
                throw;
            }
        }

        [Authorize(Roles = "Admin, Bibliotecario")]
        public IActionResult Edit(int id)
        {
            _logger.LogInformation("Accediendo a la vista de edición de la categoría con identificador {CategoriaId}", id);

            Categoria? categoria = null;

            try
            {
                using (var conexion = new SqlConnection(_connectionString))
                {
                    var sql = "SELECT ID, Nombre, Descripcion FROM Categorias WHERE ID = @ID";

                    using (var comando = new SqlCommand(sql, conexion))
                    {
                        comando.Parameters.AddWithValue("@ID", id);

                        conexion.Open();
                        using (var lector = comando.ExecuteReader())
                        {
                            if (lector.Read())
                            {
                                categoria = new Categoria
                                {
                                    ID = lector.GetInt32(0),
                                    Nombre = lector.GetString(1),
                                    Descripcion = lector.IsDBNull(2) ? null : lector.GetString(2)
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Se produjo un error al cargar la categoría {CategoriaId} para su edición", id);
                throw;
            }

            if (categoria == null)
            {
                _logger.LogWarning("No se encontró la categoría a editar con identificador {CategoriaId}", id);
                return NotFound();
            }

            return View(categoria);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Bibliotecario")]
        public IActionResult Edit(Categoria categoria)
        {
            if (categoria == null || string.IsNullOrWhiteSpace(categoria.Nombre))
            {
                _logger.LogWarning("Intento de editar una categoría sin nombre por el usuario {Usuario}", User.Identity?.Name);
                ModelState.AddModelError("Nombre", "El nombre es obligatorio.");
                return View(categoria);
            }

            _logger.LogInformation("El usuario {Usuario} intenta editar la categoría con identificador {CategoriaId}", User.Identity?.Name, categoria.ID);

            try
            {
                int filasAfectadas;

                using (var conexion = new SqlConnection(_connectionString))
                {
                    var sql = "UPDATE Categorias SET Nombre = @Nombre, Descripcion = @Descripcion WHERE ID = @ID";

                    using (var comando = new SqlCommand(sql, conexion))
                    {
                        comando.Parameters.AddWithValue("@Nombre", categoria.Nombre);
                        comando.Parameters.AddWithValue("@Descripcion", (object)categoria.Descripcion ?? System.DBNull.Value);
                        comando.Parameters.AddWithValue("@ID", categoria.ID);

                        conexion.Open();
                        filasAfectadas = comando.ExecuteNonQuery();
                    }
                }

                if (filasAfectadas == 0)
                {
                    _logger.LogWarning("No se actualizó ninguna fila: la categoría {CategoriaId} no existe.", categoria.ID);
                    return NotFound();
                }

                _logger.LogInformation("Categoría {CategoriaId} actualizada correctamente.", categoria.ID);
                TempData["SuccessMessage"] = "Categoría actualizada correctamente.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Se produjo un error al actualizar la categoría {CategoriaId}", categoria.ID);
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            _logger.LogInformation("El usuario {Usuario} intenta eliminar la categoría con identificador {CategoriaId}", User.Identity?.Name, id);

            try
            {
                int filasAfectadas;

                using (var conexion = new SqlConnection(_connectionString))
                {
                    var sql = "DELETE FROM Categorias WHERE ID = @ID";

                    using (var comando = new SqlCommand(sql, conexion))
                    {
                        comando.Parameters.AddWithValue("@ID", id);

                        conexion.Open();
                        filasAfectadas = comando.ExecuteNonQuery();
                    }
                }

                if (filasAfectadas == 0)
                {
                    _logger.LogWarning("No se eliminó ninguna fila: la categoría {CategoriaId} no existe.", id);
                    return NotFound();
                }

                _logger.LogInformation("Categoría {CategoriaId} eliminada correctamente.", id);
                TempData["SuccessMessage"] = "Categoría eliminada correctamente.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Se produjo un error al eliminar la categoría {CategoriaId}", id);
                throw;
            }
        }
    }
}
