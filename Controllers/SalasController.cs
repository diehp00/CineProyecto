using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

[Authorize(Policy = "Administrador")] // Solo para administradores
public class SalasController : Controller
{
    private readonly CineDbContext _context;

    public SalasController(CineDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var salas = _context.Salas.ToList();
        return View();
    }

    [HttpGet]
    public IActionResult Create() => View();

    [HttpPost]
    public async Task<IActionResult> Create(Sala sala)
    {
        if (ModelState.IsValid)
        {
            _context.Add(sala);
            await _context.SaveChangesAsync();
            return Redirect("http://localhost:5190/Admin/AdministradorDashboard");
        }
        return View(sala);
    }

    [HttpGet]
    public IActionResult Delete(int id)
    {
        var sala = _context.Salas.FirstOrDefault(s => s.Id == id);

        if (sala == null)
        {
            return NotFound();
        }

        return View(sala);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(Sala sala)
    {
        var salaExistente = _context.Salas.FirstOrDefault(s => s.Id == sala.Id);

        if (salaExistente == null)
        {
            return NotFound();
        }

        _context.Salas.Remove(salaExistente);
        await _context.SaveChangesAsync();

        return Redirect("http://localhost:5190/Admin/AdministradorDashboard");

    }


    [HttpGet]
    public IActionResult Edit(int id)
    {
        var sala = _context.Salas.FirstOrDefault(s => s.Id == id);

        if (sala == null)
        {
            return NotFound();
        }

        return View(sala);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Sala sala)
    {
        if (!ModelState.IsValid)
        {
            return View(sala);
        }

        var salaExistente = _context.Salas.FirstOrDefault(s => s.Id == sala.Id);

        if (salaExistente == null)
        {
            return NotFound();
        }

        // Actualizar los campos
        salaExistente.NombreSala = sala.NombreSala;
        salaExistente.Capacidad = sala.Capacidad;

        await _context.SaveChangesAsync();
        return Redirect("/Admin/GestionarSalas");
    }




}
