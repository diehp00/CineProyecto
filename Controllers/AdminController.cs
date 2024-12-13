using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

[Authorize(Policy = "Administrador")]

public class AdminController : Controller
{
    private readonly CineDbContext _context;

    public AdminController(CineDbContext context)
    {
        _context = context;
    }

    public IActionResult AdministradorDashboard()
    {
        return View();
    }

    public IActionResult GestionarPeliculas()
    {
        var peliculas = _context.Peliculas.ToList();
        return View(peliculas);
    }

    public IActionResult GestionarUsuarios()
    {
        var usuarios = _context.Usuarios.ToList();
        return View(usuarios);
    }

    public IActionResult GestionarSalas()
    {
        var salas = _context.Salas.ToList();
        return View(salas);
    }

    public IActionResult GestionarEntradas()
    {
        var entradas = _context.Entradas.ToList();
        return View(entradas);
    }

    public IActionResult GestionarHorarios()
    {
        var horarios = _context.Horarios.ToList();
        return View(horarios);
    }


    public IActionResult GestionarPromociones()
    {
        var promociones = _context.Promociones.ToList();
        return View(promociones);
    }
}
