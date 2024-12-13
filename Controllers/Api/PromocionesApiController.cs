using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Cine.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class PromocionesApiController : ControllerBase
{
    private readonly CineDbContext _context;

    public PromocionesApiController(CineDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Promocion>>> GetPromociones()
    {
        return await _context.Promociones.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Promocion>> GetPromocion(int id)
    {
        var promocion = await _context.Promociones.FindAsync(id);
        if (promocion == null) return NotFound();
        return promocion;
    }

    [HttpPost]
    public async Task<ActionResult<Promocion>> PostPromocion(Promocion promocion)
    {
        _context.Promociones.Add(promocion);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetPromocion), new { id = promocion.Id }, promocion);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePromocion(int id)
    {
        var promocion = await _context.Promociones.FindAsync(id);
        if (promocion == null) return NotFound();
        _context.Promociones.Remove(promocion);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
