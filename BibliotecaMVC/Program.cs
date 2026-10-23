using BibliotecaMVC.Repositories;
using BibliotecaMVC.Services;
using BibliotecaMVC.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<BibliotecaContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BibliotecaDB")));

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.AllowedForNewUsers = true;
}).AddEntityFrameworkStores<BibliotecaContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

builder.Services.AddScoped<IRepositorioLibro, RepositorioEnMemoria>();
builder.Services.AddScoped<IAutorService, AutorService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    // 1. Crear los roles iniciales con RoleManager, verificando que no existan (evita duplicados).
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    string[] roles = { "Admin", "Bibliotecario", "Usuario" };

    foreach (var rol in roles)
    {
        if (!await roleManager.RoleExistsAsync(rol))
        {
            var rolResult = await roleManager.CreateAsync(new IdentityRole(rol));
            if (!rolResult.Succeeded)
            {
                throw new InvalidOperationException(
                    $"No se pudo crear el rol {rol}: {string.Join(", ", rolResult.Errors.Select(error => error.Description))}"
                );
            }
        }
    }

    // 2. Asignar el rol Usuario (con UserManager) a los usuarios existentes que no tengan ningún rol.
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    var usuarios = await userManager.Users.ToListAsync();
    foreach (var usuario in usuarios)
    {
        if ((await userManager.GetRolesAsync(usuario)).Count == 0)
        {
            var result = await userManager.AddToRoleAsync(usuario, "Usuario");
            if (!result.Succeeded)
            {
                throw new InvalidOperationException(
                    $"No se pudo asignar el rol Usuario a {usuario.UserName}: {string.Join(", ", result.Errors.Select(error => error.Description))}"
                );
            }
        }
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
