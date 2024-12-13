using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity; // Necesario para PasswordHasher
using System.Threading.Tasks;

public class RegistroController : Controller
{
    private readonly CineDbContext _context;
    private readonly IPasswordHasher<Usuario> _passwordHasher;

    public RegistroController(CineDbContext context, IPasswordHasher<Usuario> passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Index(Usuario usuario)
    {
        if (ModelState.IsValid)
        {
            // Hashear la contraseña
            usuario.Contraseña = _passwordHasher.HashPassword(usuario, usuario.Contraseña);

            // Asignar el rol "Cliente" automáticamente
            usuario.Rol = "Cliente";

            // Usar una transacción para garantizar consistencia
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Agregar el usuario a la base de datos
                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();

                // Crear y asociar una promoción si el rol es "Cliente"
                if (usuario.Rol == "Cliente")
                {
                    var nuevaPromocion = new Promocion
                    {
                        Descripcion = "Descuento del 20% en entradas",
                        FechaInicio = DateTime.Now,
                        FechaFin = DateTime.Now.AddDays(7), // Promoción válida por una semana
                        Descuento = 20, // 20% de descuento
                        UsuarioId = usuario.Id // Asociar la promoción al usuario recién registrado
                    };

                    _context.Promociones.Add(nuevaPromocion);
                    await _context.SaveChangesAsync();

                    // Asociar la promoción al usuario
                    usuario.PromocionID = nuevaPromocion.Id;
                    _context.Usuarios.Update(usuario);
                    await _context.SaveChangesAsync();
                }

                // Confirmar la transacción
                await transaction.CommitAsync();

                // Redirigir al login después del registro
                return RedirectToAction("Index", "Login");
            }
            catch
            {
                // Si algo falla, revertir la transacción
                await transaction.RollbackAsync();
                ModelState.AddModelError("", "Ocurrió un error al registrar el usuario.");
            }   
        }

        // Si el modelo no es válido o algo falla, devolver la vista con los datos actuales
        return View(usuario);
    }
}
