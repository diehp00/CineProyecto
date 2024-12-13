using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Cine.Models;
using Microsoft.AspNetCore.Identity;

using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);


// Configurar conexión a la base de datos MySQL
builder.Services.AddDbContext<CineDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("CineDB"), 
        new MySqlServerVersion(new Version(8, 0, 21))));


builder.Services.AddDbContext<CineDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("CineDB"),
        new MySqlServerVersion(new Version(8, 0, 21)),
        mysqlOptions => mysqlOptions.EnableRetryOnFailure())
);


// Configuración de autenticación con cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login/Index";
    });

    

// Configuración de autorización
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Administrador", policy => policy.RequireRole("Administrador"));
});

// Habilitar controladores de vistas y API
builder.Services.AddControllersWithViews();
builder.Services.AddControllers(); // Habilita los controladores API

// Registrar el servicio de PasswordHasher
builder.Services.AddScoped<IPasswordHasher<Usuario>, PasswordHasher<Usuario>>();

// Configuración de Swagger para la documentación de la API
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "Cine API", 
        Version = "v1",
        Description = "API para la aplicación de cine con operaciones CRUD para películas, entradas, usuarios, salas y promociones",
        Contact = new OpenApiContact
        {
            Name = "Soporte Cine",
            Email = "soporte@cine.com"
        }
    });
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    // Habilitar Swagger en desarrollo
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Cine API v1"));
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Configuración de autenticación y autorización
app.UseAuthentication();
app.UseAuthorization();

// Configurar rutas para controladores de vistas y API
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Asegurar que los controladores de API usen las rutas apropiadas
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    try
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<CineDbContext>();
        dbContext.Database.Migrate();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error al aplicar migraciones: {ex.Message}");
    }
}

app.Run();
