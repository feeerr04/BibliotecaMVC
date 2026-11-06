using System.ComponentModel.DataAnnotations;
using BibliotecaMVC.Models;

namespace BibliotecaMVC.Tests;

// Pruebas de las reglas de validación (Data Annotations) del modelo Libro.
public class LibroValidationTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void TituloEsObligatorio(string? titulo)
    {
        var errores = Validate(new Libro { Titulo = titulo!, Autor = "Autor" });
        Assert.Contains(errores, e => e.MemberNames.Contains(nameof(Libro.Titulo)));
    }

    [Theory]
    [InlineData(150, false)]
    [InlineData(151, true)]
    public void TituloRespetaElLimiteDe150Caracteres(int longitud, bool debeFallar)
    {
        var errores = Validate(new Libro { Titulo = new string('T', longitud), Autor = "Autor" });
        Assert.Equal(debeFallar, errores.Any(e => e.MemberNames.Contains(nameof(Libro.Titulo))));
    }

    [Fact]
    public void AutorEsObligatorio()
    {
        var errores = Validate(new Libro { Titulo = "Título", Autor = string.Empty });
        Assert.Contains(errores, e => e.MemberNames.Contains(nameof(Libro.Autor)));
    }

    [Theory]
    [InlineData(-1, true)]
    [InlineData(0, false)]
    [InlineData(9999, false)]
    [InlineData(10000, true)]
    public void PrecioDebeEstarEntre0Y9999(int precio, bool debeFallar)
    {
        var errores = Validate(new Libro { Titulo = "Título", Autor = "Autor", Precio = precio });
        Assert.Equal(debeFallar, errores.Any(e => e.MemberNames.Contains(nameof(Libro.Precio))));
    }

    [Fact]
    public void LibroCompletoEsValido()
    {
        var errores = Validate(new Libro { Titulo = "Ficciones", Autor = "Borges", Categoria = "Cuento", Precio = 10 });
        Assert.Empty(errores);
    }

    private static List<ValidationResult> Validate(Libro libro)
    {
        var errores = new List<ValidationResult>();
        Validator.TryValidateObject(libro, new ValidationContext(libro), errores, validateAllProperties: true);
        return errores;
    }
}
