using BibliotecaMVC.Controllers;
using BibliotecaMVC.Data;
using BibliotecaMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace BibliotecaMVC.Tests;

// Pruebas unitarias del módulo Libros (EF Core InMemory + Moq para el logger).
public class LibrosControllerTests
{
    [Fact]
    public async Task Index_DevuelveLaListaDeLibros()
    {
        await using var context = CreateContext();
        context.Libros.AddRange(
            new Libro { Titulo = "Cien años de soledad", Autor = "García Márquez", Categoria = "Novela", ImagenUrl = "img.jpg" },
            new Libro { Titulo = "La casa de los espíritus", Autor = "Isabel Allende", Categoria = "Novela", ImagenUrl = "img.jpg" });
        await context.SaveChangesAsync();
        var controller = CreateController(context, out _);

        var result = await controller.Index();

        var view = Assert.IsType<ViewResult>(result);
        var libros = Assert.IsAssignableFrom<IEnumerable<Libro>>(view.Model);
        Assert.Equal(2, libros.Count());
    }

    [Fact]
    public async Task Index_SinLibros_DevuelveListaVacia()
    {
        await using var context = CreateContext();
        var controller = CreateController(context, out _);

        var result = await controller.Index();

        var view = Assert.IsType<ViewResult>(result);
        Assert.Empty(Assert.IsAssignableFrom<IEnumerable<Libro>>(view.Model));
    }

    [Fact]
    public async Task Details_ConLibroExistente_DevuelveElLibro()
    {
        await using var context = CreateContext();
        var libro = new Libro { Titulo = "Rayuela", Autor = "Cortázar", Categoria = "Novela", ImagenUrl = "img.jpg" };
        context.Libros.Add(libro);
        await context.SaveChangesAsync();
        var controller = CreateController(context, out _);

        var result = await controller.Details(libro.Id);

        var view = Assert.IsType<ViewResult>(result);
        Assert.Same(libro, view.Model);
    }

    [Fact]
    public async Task Details_ConLibroInexistente_DevuelveNotFoundYRegistraAdvertencia()
    {
        await using var context = CreateContext();
        var controller = CreateController(context, out var logger);

        var result = await controller.Details(999);

        Assert.IsType<NotFoundResult>(result);
        // Moq: verifica que se escribió exactamente un log de nivel Warning.
        VerifyLog(logger, LogLevel.Warning, Times.Once());
    }

    [Fact]
    public async Task Create_ConLibroValido_GuardaYRedirigeAlIndice()
    {
        await using var context = CreateContext();
        var controller = CreateController(context, out _);
        var libro = new Libro { Titulo = "El Aleph", Autor = "Borges", Precio = 12.5m, Disponible = true, Categoria = "Novela", ImagenUrl = "img.jpg" };

        var result = await controller.Create(libro);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(LibrosController.Index), redirect.ActionName);
        Assert.Equal(1, await context.Libros.CountAsync());
        Assert.Equal("El Aleph", (await context.Libros.SingleAsync()).Titulo);
    }

    [Fact]
    public async Task Create_ConModeloInvalido_NoGuardaYDevuelveLaVista()
    {
        await using var context = CreateContext();
        var controller = CreateController(context, out _);
        controller.ModelState.AddModelError(nameof(Libro.Titulo), "El título es obligatorio.");
        var libro = new Libro { Autor = "Borges", Categoria = "Novela", ImagenUrl = "img.jpg" };

        var result = await controller.Create(libro);

        var view = Assert.IsType<ViewResult>(result);
        Assert.Same(libro, view.Model);
        Assert.Empty(context.Libros);
    }

    [Fact]
    public async Task Edit_ConLibroValido_ActualizaElLibro()
    {
        var db = Guid.NewGuid().ToString();
        int id;
        await using (var seed = CreateContext(db))
        {
            var libro = new Libro { Titulo = "Título original", Autor = "Autor", Categoria = "Novela", ImagenUrl = "img.jpg" };
            seed.Libros.Add(libro);
            await seed.SaveChangesAsync();
            id = libro.Id;
        }
        await using var context = CreateContext(db);
        var controller = CreateController(context, out _);

        var result = await controller.Edit(id, new Libro { Id = id, Titulo = "Título nuevo", Autor = "Autor", Categoria = "Novela", ImagenUrl = "img.jpg" });

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(LibrosController.Index), redirect.ActionName);
        Assert.Equal("Título nuevo", (await context.Libros.AsNoTracking().SingleAsync()).Titulo);
    }

    [Fact]
    public async Task Edit_ConIdentificadorDiferente_DevuelveBadRequest()
    {
        await using var context = CreateContext();
        var controller = CreateController(context, out _);

        var result = await controller.Edit(2, new Libro { Id = 1, Titulo = "X", Autor = "Y", Categoria = "Novela", ImagenUrl = "img.jpg" });

        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task Edit_ConLibroInexistente_DevuelveNotFound()
    {
        await using var context = CreateContext();
        var controller = CreateController(context, out _);

        var result = await controller.Edit(999, new Libro { Id = 999, Titulo = "X", Autor = "Y", Categoria = "Novela", ImagenUrl = "img.jpg" });

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Delete_ConLibroExistente_EliminaYRedirige()
    {
        await using var context = CreateContext();
        var libro = new Libro { Titulo = "Para eliminar", Autor = "Autor", Categoria = "Novela", ImagenUrl = "img.jpg" };
        context.Libros.Add(libro);
        await context.SaveChangesAsync();
        var controller = CreateController(context, out var logger);

        var result = await controller.Delete(libro.Id);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(LibrosController.Index), redirect.ActionName);
        Assert.Empty(context.Libros);
        VerifyLog(logger, LogLevel.Information, Times.AtLeastOnce());
    }

    [Fact]
    public async Task Delete_ConLibroInexistente_DevuelveNotFound()
    {
        await using var context = CreateContext();
        var controller = CreateController(context, out _);

        var result = await controller.Delete(999);

        Assert.IsType<NotFoundResult>(result);
    }

    private static LibrosController CreateController(BibliotecaContext context, out Mock<ILogger<LibrosController>> logger)
    {
        logger = new Mock<ILogger<LibrosController>>();
        return new LibrosController(context, logger.Object);
    }

    // ILogger.Log es el método real detrás de LogWarning/LogInformation/etc.
    private static void VerifyLog(Mock<ILogger<LibrosController>> logger, LogLevel level, Times times) =>
        logger.Verify(l => l.Log(
            level,
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception?>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), times);

    private static BibliotecaContext CreateContext(string? databaseName = null)
    {
        var options = new DbContextOptionsBuilder<BibliotecaContext>()
            .UseInMemoryDatabase(databaseName ?? Guid.NewGuid().ToString())
            .Options;
        return new BibliotecaContext(options);
    }
}
