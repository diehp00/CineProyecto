using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Linq;

public class LoginController : Controller
{
    private readonly CineDbContext _context;
    private readonly IPasswordHasher<Usuario> _passwordHasher;

    public LoginController(CineDbContext context, IPasswordHasher<Usuario> passwordHasher)
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
    public async Task<IActionResult> Index(string nombreUsuario, string contraseña)
    {
        // Busca al usuario en la base de datos
        var usuario = _context.Usuarios.FirstOrDefault(u => u.NombreUsuario == nombreUsuario);

        if (usuario == null)
        {
            // Si no encuentra al usuario, redirige a la página de registro
            TempData["Error"] = "Usuario no encontrado. Por favor, regístrate.";
            return RedirectToAction("Index", "Registro");
        }

        // Verifica la contraseña si el usuario existe
        var resultado = _passwordHasher.VerifyHashedPassword(usuario, usuario.Contraseña, contraseña);
        if (resultado == PasswordVerificationResult.Success)
        {
            // Si la contraseña es correcta, crea los claims para la autenticación
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario.NombreUsuario),
                new Claim(ClaimTypes.Role, usuario.Rol),
                new Claim("UsuarioId", usuario.Id.ToString()) // Añade el UsuarioId como claim
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties();

            // Inicia sesión
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

            // Redirige al Home
            return RedirectToAction("Index", "Home");
        }

        // Si la contraseña es incorrecta, muestra un mensaje de error
        ModelState.AddModelError("", "La contraseña es incorrecta.");
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }
}
