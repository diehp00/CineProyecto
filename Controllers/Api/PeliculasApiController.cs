using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Cine.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class PeliculasApiController : ControllerBase
{
    private readonly CineDbContext _context;

    public PeliculasApiController(CineDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Pelicula>>> GetPeliculas()
    {
        return await _context.Peliculas.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Pelicula>> GetPelicula(int id)
    {
        var pelicula = await _context.Peliculas.FindAsync(id);
        if (pelicula == null) return NotFound();
        return pelicula;
    }

    [HttpPost]
    public async Task<ActionResult<Pelicula>> PostPelicula(Pelicula pelicula)
    {
        _context.Peliculas.Add(pelicula);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetPelicula), new { id = pelicula.Id }, pelicula);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutPelicula(int id, Pelicula pelicula)
    {
        if (id != pelicula.Id) return BadRequest();
        _context.Entry(pelicula).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePelicula(int id)
    {
        var pelicula = await _context.Peliculas.FindAsync(id);
        if (pelicula == null) return NotFound();
        _context.Peliculas.Remove(pelicula);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
