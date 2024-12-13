using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

public class HorariosController : Controller
{
    private readonly CineDbContext _context;

    public HorariosController(CineDbContext context)
    {
        _context = context;
    }

    // Acción para listar horarios
    public IActionResult Index()
    {
        var horarios = _context.Horarios.ToList();
        return View(horarios);
    }

    [HttpGet]
    public IActionResult Create()
    {
        var salas = _context.Salas.ToList(); // Obtén la lista de salas

        if (!salas.Any())
        {
            ModelState.AddModelError("", "No hay salas disponibles. Por favor, cree salas antes de añadir un horario.");
            return View();
        }

        // Convierte las salas en una colección de SelectListItem
        ViewBag.Salas = salas.Select(s => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
        {
            Value = s.Id.ToString(), // ID como valor
            Text = s.NombreSala      // NombreSala como texto visible
        }).ToList();

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Horario horario)
    {
        if (!ModelState.IsValid)
        {
            // Recargar ViewBag.Salas en caso de error de validación
            var salas = _context.Salas.ToList();
            ViewBag.Salas = salas.Select(s => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Value = s.Id.ToString(),
                Text = s.NombreSala
            }).ToList();
            return View(horario);
        }

        // Validar si ya hay 3 horarios en el mismo día para la misma sala
        var fechaDelHorario = horario.FechaHora.Date; // Solo la fecha sin la hora
        var horariosExistentes = _context.Horarios
            .Where(h => h.SalaId == horario.SalaId && h.FechaHora.Date == fechaDelHorario)
            .ToList();

        if (horariosExistentes.Count >= 3)
        {
            ModelState.AddModelError("", "No se pueden agregar más de 3 horarios en un mismo día para esta sala.");
            var salas = _context.Salas.ToList();
            ViewBag.Salas = salas.Select(s => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Value = s.Id.ToString(),
                Text = s.NombreSala
            }).ToList();
            return View(horario);
        }

        // Validar que no haya conflictos de 2 horas entre horarios
        var conflictoHorario = horariosExistentes
            .Any(h => Math.Abs((h.FechaHora - horario.FechaHora).TotalHours) < 2);

        if (conflictoHorario)
        {
            ModelState.AddModelError("", "Ya existe un horario cercano en la misma sala. Debe haber al menos 2 horas de diferencia.");
            var salas = _context.Salas.ToList();
            ViewBag.Salas = salas.Select(s => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Value = s.Id.ToString(),
                Text = s.NombreSala
            }).ToList();
            return View(horario);
        }

        try
        {
            _context.Horarios.Add(horario);
            await _context.SaveChangesAsync();
            return Redirect("/Admin/GestionarHorarios");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al guardar el horario: {ex.Message}");
            ModelState.AddModelError("", "Ocurrió un error al guardar el horario.");
            var salas = _context.Salas.ToList();
            ViewBag.Salas = salas.Select(s => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Value = s.Id.ToString(),
                Text = s.NombreSala
            }).ToList();
            return View(horario);
        }
    }

    [HttpGet]
    public IActionResult Delete(int id)
    {
        var horario = _context.Horarios.FirstOrDefault(p => p.Id == id);

        if (horario == null)
        {
            return NotFound();
        }

        return View(horario);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(Horario horario)
    {
        var horarioExistente = _context.Horarios.FirstOrDefault(p => p.Id == horario.Id);

        if (horarioExistente == null)
        {
            return NotFound();
        }

        _context.Horarios.Remove(horarioExistente);
        await _context.SaveChangesAsync();

        return Redirect("/Admin/GestionarHorarios");
    }


    [HttpGet]
    public IActionResult Edit(int id)
    {
        var horario = _context.Horarios.FirstOrDefault(p => p.Id == id);

        if (horario == null)
        {
            return NotFound();
        }

        ViewBag.Salas = _context.Salas.ToList();
        return View(horario);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Horario horario)
    {
        if (!ModelState.IsValid)
        {
            // Log para identificar errores
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine($"Error: {error.ErrorMessage}");
            }

            ViewBag.Salas = _context.Salas.ToList();
            return View(horario);
        }

        var horarioExistente = _context.Horarios.FirstOrDefault(p => p.Id == horario.Id);
        if (horarioExistente == null)
        {
            return NotFound();
        }

        // Actualizar propiedades
        horarioExistente.FechaHora = horario.FechaHora;
        horarioExistente.SalaId = horario.SalaId;

        await _context.SaveChangesAsync();

        return Redirect("/Admin/GestionarHorarios");
    }

}
