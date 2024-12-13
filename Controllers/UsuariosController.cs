using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

public class UsuariosController : Controller
{
    private readonly CineDbContext _context;
    private readonly IPasswordHasher<Usuario> _passwordHasher;

    public UsuariosController(CineDbContext context, IPasswordHasher<Usuario> passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    [HttpGet]
    public IActionResult Registro() => View();

    [HttpPost]
    public async Task<IActionResult> Registro(Usuario usuario)
    {
        if (ModelState.IsValid)
        {
            // Hashear la contraseña antes de guardarla
            usuario.Contraseña = _passwordHasher.HashPassword(usuario, usuario.Contraseña);

            _context.Add(usuario);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Login));
        }
        return View(usuario);
    }

    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    public async Task<IActionResult> Login(string nombreUsuario, string contraseña)
    {
        var usuario = _context.Usuarios.FirstOrDefault(u => u.NombreUsuario == nombreUsuario);

        if (usuario == null || _passwordHasher.VerifyHashedPassword(usuario, usuario.Contraseña, contraseña) != PasswordVerificationResult.Success)
        {
            ModelState.AddModelError("", "Nombre de usuario o contraseña incorrectos.");
            return View();
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, usuario.NombreUsuario),
            new Claim(ClaimTypes.Role, usuario.Rol)
        };

        var claimsIdentity = new ClaimsIdentity(claims, "CookieAuth");
        await HttpContext.SignInAsync("CookieAuth", new ClaimsPrincipal(claimsIdentity));

        return RedirectToAction("Index", "Home");
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Delete(int id)
    {
        var usuario = _context.Usuarios.FirstOrDefault(p => p.Id == id);

        if (usuario == null)
        {
            return NotFound();
        }

        return View(usuario);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(Usuario usuario)
    {
        var usuarioExistente = _context.Usuarios.FirstOrDefault(p => p.Id == usuario.Id);

        if (usuarioExistente == null)
        {
            return NotFound();
        }

        _context.Usuarios.Remove(usuarioExistente);
        await _context.SaveChangesAsync();
        return Redirect("/Admin/GestionarUsuarios");
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var usuario = _context.Usuarios.FirstOrDefault(u => u.Id == id);

        if (usuario == null)
        {
            return NotFound();
        }

        return View(usuario);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Usuario usuario)
    {
        if (!ModelState.IsValid)
        {
            return View(usuario);
        }

        var usuarioExistente = _context.Usuarios.FirstOrDefault(u => u.Id == usuario.Id);

        if (usuarioExistente == null)
        {
            return NotFound();
        }

        // Actualizar los campos del usuario
        usuarioExistente.NombreUsuario = usuario.NombreUsuario;

        // Hashear la contraseña solo si se proporciona una nueva
        if (!string.IsNullOrWhiteSpace(usuario.Contraseña))
        {
            usuarioExistente.Contraseña = _passwordHasher.HashPassword(usuario, usuario.Contraseña);
        }

        usuarioExistente.Rol = usuario.Rol;
        usuarioExistente.CorreoElectronico = usuario.CorreoElectronico;

        // Guardar los cambios en la base de datos
        await _context.SaveChangesAsync();

        return Redirect("/Admin/GestionarUsuarios");
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Perfil()
    {
        var nombreUsuario = User.Identity.Name;

        if (string.IsNullOrEmpty(nombreUsuario))
        {
            return RedirectToAction("Login", "Usuarios");
        }

        var usuario = await _context.Usuarios
            .Include(u => u.Entradas)
            .ThenInclude(e => e.Pelicula)
            .FirstOrDefaultAsync(u => u.NombreUsuario == nombreUsuario);

        var usuarioP = await _context.Usuarios
            .Include(u => u.Promocion)
            .FirstOrDefaultAsync(u => u.NombreUsuario == nombreUsuario);

        if (usuario == null)
        {
            return NotFound("Usuario no encontrado.");
        }

        if (usuarioP == null)
        {
            return NotFound("Promoción no encontrada.");
        }

        return View(usuario);
    }
}
