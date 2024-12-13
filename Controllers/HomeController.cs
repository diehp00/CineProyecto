using Microsoft.AspNetCore.Mvc;
using System.Linq;

public class HomeController : Controller
{
    private readonly CineDbContext _context;

    public HomeController(CineDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        // Obtener las películas en cartelera (puedes aplicar algún filtro o limitar el número de películas)
        var peliculas = _context.Peliculas.ToList();
        return View(peliculas);
    }

    public IActionResult Contacto()
{
    return View();
}

}
