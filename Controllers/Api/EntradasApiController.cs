using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Cine.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class EntradasApiController : ControllerBase
{
    private readonly CineDbContext _context;

    public EntradasApiController(CineDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Entrada>>> GetEntradas()
    {
        return await _context.Entradas.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Entrada>> GetEntrada(int id)
    {
        var entrada = await _context.Entradas.FindAsync(id);
        if (entrada == null) return NotFound();
        return entrada;
    }

    [HttpPost]
    public async Task<ActionResult<Entrada>> PostEntrada(Entrada entrada)
    {
        _context.Entradas.Add(entrada);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetEntrada), new { id = entrada.Id }, entrada);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEntrada(int id)
    {
        var entrada = await _context.Entradas.FindAsync(id);
        if (entrada == null) return NotFound();
        _context.Entradas.Remove(entrada);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
