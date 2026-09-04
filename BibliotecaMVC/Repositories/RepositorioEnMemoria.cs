using BibliotecaMVC.Models;

namespace BibliotecaMVC.Repositories
{
    public class RepositorioEnMemoria : IRepositorioLibro
    {
        private static List<Libro> _libros = new List<Libro>()
        {
            new Libro { Id = 1, Titulo = "Clean Code", Autor = "Robert C. Martin", Categoria = "Programación", Precio = 35.5m, Disponible = true, ImagenUrl = "clean-code.jpg" },
            new Libro { Id = 2, Titulo = "The Pragmatic Programmer", Autor = "Andrew Hunt", Categoria = "Programación", Precio = 40.0m, Disponible = false, ImagenUrl = "pragmatic.jpg" }
        };

        public IEnumerable<Libro> ObtenerTodos()
        {
            return _libros;
        }

        public Libro? ObtenerPorId(int id)
        {
            return _libros.FirstOrDefault(l => l.Id == id);
        }

        public void Agregar(Libro libro)
        {
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
        }

        public void Actualizar(Libro libroEditado)
        {
            var libro = _libros.FirstOrDefault(l => l.Id == libroEditado.Id);

            if (libro != null)
            {
                libro.Titulo = libroEditado.Titulo;
                libro.Autor = libroEditado.Autor;
                libro.Categoria = libroEditado.Categoria;
                libro.Precio = libroEditado.Precio;
                libro.Disponible = libroEditado.Disponible;
            }
        }

        public void Eliminar(int id)
        {
            var libro = _libros.FirstOrDefault(l => l.Id == id);

            if (libro != null)
            {
                _libros.Remove(libro);
            }
        }
    }
}
