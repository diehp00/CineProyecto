using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Cine.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = "Administrador")]
public class AdminApiController : ControllerBase
{
    private readonly CineDbContext _context;

    public AdminApiController(CineDbContext context)
    {
        _context = context;
    }

    // Listar todos los usuarios
    [HttpGet("usuarios")]
    public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
    {
        return await _context.Usuarios.ToListAsync();
    }

    // Asignar un rol a un usuario
    [HttpPut("usuarios/{id}/rol")]
    public async Task<IActionResult> AsignarRol(int id, [FromBody] string rol)
    {
        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario == null) return NotFound();

        usuario.Rol = rol;
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
