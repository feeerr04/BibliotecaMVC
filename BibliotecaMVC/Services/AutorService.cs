using BibliotecaMVC.Models;

namespace BibliotecaMVC.Services
{
    public class AutorService : IAutorService
    {
        private static List<Autor> _autores = new List<Autor>
        {
            new Autor { Id = 1, Nombre = "Gabriel García Márquez", Nacionalidad = "Colombiana", FechaNacimiento = new DateTime(1927, 3, 6) },
            new Autor { Id = 2, Nombre = "Isabel Allende", Nacionalidad = "Chilena", FechaNacimiento = new DateTime(1942, 8, 2) },
            new Autor { Id = 3, Nombre = "Mario Vargas Llosa", Nacionalidad = "Peruana", FechaNacimiento = new DateTime(1936, 3, 28) }
        };

        public IEnumerable<Autor> ObtenerTodos()
        {
            return _autores;
        }

        public Autor? ObtenerPorId(int id)
        {
            return _autores.FirstOrDefault(a => a.Id == id);
        }

        public void Agregar(Autor autor)
        {
            if (_autores.Any())
            {
                autor.Id = _autores.Max(x => x.Id) + 1;
            }
            else
            {
                autor.Id = 1;
            }

            _autores.Add(autor);
        }

        public void Actualizar(Autor autorEditado)
        {
            var autor = _autores.FirstOrDefault(a => a.Id == autorEditado.Id);

            if (autor != null)
            {
                autor.Nombre = autorEditado.Nombre;
                autor.Nacionalidad = autorEditado.Nacionalidad;
                autor.FechaNacimiento = autorEditado.FechaNacimiento;
            }
        }

        public void Eliminar(int id)
        {
            var autor = _autores.FirstOrDefault(a => a.Id == id);

            if (autor != null)
            {
                _autores.Remove(autor);
            }
        }
    }
}
