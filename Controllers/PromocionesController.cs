using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;


[Authorize(Roles = "Administrador,Cliente")]
public class PromocionesController : Controller
{
    private readonly CineDbContext _context;

    public PromocionesController(CineDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        // Obtener el UsuarioId del usuario autenticado
        var usuarioIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UsuarioId");

        if (usuarioIdClaim == null)
        {
            return Unauthorized(); // Redirige si no hay un UsuarioId
        }

        int usuarioId = int.Parse(usuarioIdClaim.Value);

        // Filtrar promociones por UsuarioId
        var promociones = _context.Promociones
            .Where(p => p.UsuarioId == usuarioId)
            .ToList();

        return View(promociones);
    }



    [Authorize(Roles = "Administrador")]
    [HttpGet]
    public IActionResult Create()
    {
        ViewData["Usuarios"] = _context.Usuarios
            .Select(u => new SelectListItem
            {
                Value = u.Id.ToString(),
                Text = u.NombreUsuario
            })
            .ToList();

        return View();
    }

    [Authorize(Roles = "Administrador")]
    [HttpPost]
    public async Task<IActionResult> Create(Promocion promocion)
    {
        // Validar que la fecha de fin sea posterior a la fecha de inicio
        if (promocion.FechaFin <= promocion.FechaInicio)
        {
            ModelState.AddModelError("FechaFin", "La fecha de finalización debe ser posterior a la fecha de inicio.");
        }

        if (ModelState.IsValid)
        {
            // Verificar si el usuario ya tiene 2 promociones
            var promocionesDelUsuario = _context.Promociones
                .Where(p => p.UsuarioId == promocion.UsuarioId)
                .Count();

            if (promocionesDelUsuario >= 2)
            {
                // Mostrar un mensaje de error en el modelo
                ModelState.AddModelError("", "El usuario ya tiene 2 promociones asignadas. No se le puede asignar más.");

                // Recargar la lista de usuarios para volver a renderizar la vista
                ViewData["Usuarios"] = _context.Usuarios
                    .Select(u => new SelectListItem
                    {
                        Value = u.Id.ToString(),
                        Text = u.NombreUsuario
                    })
                    .ToList();

                return View(promocion);
            }

            // Si no tiene 2 promociones, guardar la nueva promoción
            try
            {
                _context.Promociones.Add(promocion);
                await _context.SaveChangesAsync();
                return Redirect("/Admin/GestionarPromociones");
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException is MySqlException sqlEx && sqlEx.Message.Contains("Duplicate entry"))
                {
                    ModelState.AddModelError("", "Ya existe una promoción con este usuario. Verifique los datos ingresados.");
                }
                else
                {
                    ModelState.AddModelError("", "Ocurrió un error al guardar los datos.");
                }

                // Recargar usuarios
                ViewData["Usuarios"] = _context.Usuarios
                    .Select(u => new SelectListItem
                    {
                        Value = u.Id.ToString(),
                        Text = u.NombreUsuario
                    })
                    .ToList();

                return View(promocion);
            }
        }

        // Recargar usuarios si hay errores de validación en el modelo
        ViewData["Usuarios"] = _context.Usuarios
            .Select(u => new SelectListItem
            {
                Value = u.Id.ToString(),
                Text = u.NombreUsuario
            })
            .ToList();

        return View(promocion);
    }

    // Editar una promoción existente (solo para administradores)
    [Authorize(Roles = "Administrador")]
    [HttpGet]
    public IActionResult Edit(int id)
    {
        var promocion = _context.Promociones.FirstOrDefault(p => p.Id == id);
        if (promocion == null)
        {
            return NotFound();
        }

        ViewData["Usuarios"] = new SelectList(_context.Usuarios, "Id", "NombreUsuario", promocion.UsuarioId);
        return View(promocion);
    }

    [Authorize(Roles = "Administrador")]
    [HttpPost]
    public async Task<IActionResult> Edit(int id, Promocion promocion)
    {
        if (id != promocion.Id)
        {
            return BadRequest();
        }

        // Validar que la fecha de fin sea posterior a la fecha de inicio
        if (promocion.FechaFin <= promocion.FechaInicio)
        {
            ModelState.AddModelError("FechaFin", "La fecha de finalización debe ser posterior a la fecha de inicio.");
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Promociones.Update(promocion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al actualizar la promoción.");
            }
        }

        ViewData["Usuarios"] = new SelectList(_context.Usuarios, "Id", "NombreUsuario", promocion.UsuarioId);
        return View(promocion);
    }

    [HttpGet]
    public IActionResult Delete(int id)
    {
        var promociones = _context.Promociones.FirstOrDefault(p => p.Id == id);

        if (promociones == null)
        {
            return NotFound();
        }

        return View(promociones);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(Promocion pomociones)
    {
        var promocionExistente = _context.Promociones.FirstOrDefault(p => p.Id == pomociones.Id);

        if (promocionExistente == null)
        {
            return NotFound();
        }

        _context.Promociones.Remove(promocionExistente);
        await _context.SaveChangesAsync();

        return Redirect("/Admin/GestionarPromociones");
    }
}
