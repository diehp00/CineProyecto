using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Cine.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class SalasApiController : ControllerBase
{
    private readonly CineDbContext _context;

    public SalasApiController(CineDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Sala>>> GetSalas()
    {
        return await _context.Salas.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Sala>> GetSala(int id)
    {
        var sala = await _context.Salas.FindAsync(id);
        if (sala == null) return NotFound();
        return sala;
    }

    [HttpPost]
    public async Task<ActionResult<Sala>> PostSala(Sala sala)
    {
        _context.Salas.Add(sala);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetSala), new { id = sala.Id }, sala);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutSala(int id, Sala sala)
    {
        if (id != sala.Id) return BadRequest();
        _context.Entry(sala).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSala(int id)
    {
        var sala = await _context.Salas.FindAsync(id);
        if (sala == null) return NotFound();
        _context.Salas.Remove(sala);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
