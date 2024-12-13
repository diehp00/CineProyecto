using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

[Authorize(Policy = "Administrador")]
public class PeliculasController : Controller
{
    private readonly CineDbContext _context;

    public PeliculasController(CineDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var peliculas = _context.Peliculas.ToList();
        return View(peliculas);
    }

    [HttpGet]
    public IActionResult Create()
    {
        ViewBag.Salas = _context.Salas.ToList();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Pelicula pelicula, [FromForm] int salaId)
    {
        // Verificar si la sala ya tiene una película asignada
        var salaConPelicula = _context.Peliculas.FirstOrDefault(p => p.SalaId == salaId);
        if (salaConPelicula != null)
        {
            ModelState.AddModelError("SalaId", "La sala seleccionada ya tiene una película asignada.");
            ViewBag.Salas = _context.Salas.ToList();
            return View(pelicula);
        }

        // Asignar SalaId a la película
        pelicula.SalaId = salaId;

        if (!ModelState.IsValid)
        {
            ViewBag.Salas = _context.Salas.ToList();
            return View(pelicula);
        }

        _context.Peliculas.Add(pelicula);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var pelicula = _context.Peliculas.FirstOrDefault(p => p.Id == id);

        if (pelicula == null)
        {
            return NotFound();
        }

        ViewBag.Salas = _context.Salas.ToList();
        return View(pelicula);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Pelicula pelicula)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Salas = _context.Salas.ToList();
            return View(pelicula);
        }

        // Verificar si la sala ya tiene una película asignada y no es la actual
        var salaConPelicula = _context.Peliculas.FirstOrDefault(p => p.SalaId == pelicula.SalaId && p.Id != pelicula.Id);
        if (salaConPelicula != null)
        {
            ModelState.AddModelError("SalaId", "La sala seleccionada ya tiene una película asignada.");
            ViewBag.Salas = _context.Salas.ToList();
            return View(pelicula);
        }

        var peliculaExistente = _context.Peliculas.FirstOrDefault(p => p.Id == pelicula.Id);
        if (peliculaExistente == null)
        {
            return NotFound();
        }

        // Actualiza las propiedades de la película existente
        peliculaExistente.Titulo = pelicula.Titulo;
        peliculaExistente.Descripcion = pelicula.Descripcion;
        peliculaExistente.Genero = pelicula.Genero;
        peliculaExistente.FechaEstreno = pelicula.FechaEstreno;
        peliculaExistente.SalaId = pelicula.SalaId;
        peliculaExistente.ImagenUrl = pelicula.ImagenUrl;
        peliculaExistente.TrailerUrl = pelicula.TrailerUrl;
        peliculaExistente.Valoracion = pelicula.Valoracion;

        _context.Peliculas.Update(peliculaExistente);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
    [HttpGet]
    public IActionResult Delete(int id)
    {
        var pelicula = _context.Peliculas.FirstOrDefault(p => p.Id == id);

        if (pelicula == null)
        {
            return NotFound();
        }

        return View(pelicula);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(Pelicula pelicula)
    {
        var peliculaExistente = _context.Peliculas.FirstOrDefault(p => p.Id == pelicula.Id);

        if (peliculaExistente == null)
        {
            return NotFound();
        }

        _context.Peliculas.Remove(peliculaExistente);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

}
