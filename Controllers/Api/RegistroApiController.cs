using Microsoft.AspNetCore.Mvc;
using Cine.Models;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class RegistroApiController : ControllerBase
{
    private readonly CineDbContext _context;

    public RegistroApiController(CineDbContext context)
    {
        _context = context;
    }

    // Registrar un nuevo usuario
    [HttpPost]
    public async Task<ActionResult<Usuario>> RegistrarUsuario(Usuario usuario)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        usuario.Rol = "Cliente";
        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(RegistrarUsuario), new { id = usuario.Id }, usuario);
    }
}
