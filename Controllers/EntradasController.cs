using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRCoder;
using System.Drawing.Imaging;
using MailKit.Net.Smtp;
using MimeKit;

using Newtonsoft.Json;






public class EntradasController : Controller
{

    private readonly CineDbContext _context;

    public EntradasController(CineDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Route("Entradas/Comprar/{peliculaId:int}")]
    public IActionResult Comprar(int peliculaId)
    {
        // Obtener la película y su sala
        var pelicula = _context.Peliculas.Include(p => p.Sala).FirstOrDefault(p => p.Id == peliculaId);
        if (pelicula == null || pelicula.Sala == null)
        {
            return NotFound(); // Devuelve 404 si no encuentra la película o sala
        }



        // Obtener los horarios disponibles para la sala de la película
        var horarios = _context.Horarios
            .Where(h => h.SalaId == pelicula.SalaId)
            .ToList();

        // Lista de promociones, vacía por defecto
        List<Promocion> promocionesUsuario = new List<Promocion>();

        // Verificar si el usuario está autenticado y si no es temporal
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            var usuarioLogueado = _context.Usuarios.FirstOrDefault(u => u.NombreUsuario == User.Identity.Name);
            if (usuarioLogueado != null && !usuarioLogueado.EsTemporal)
            {
                // Cargar promociones solo si el usuario no es temporal
                promocionesUsuario = _context.Promociones
                    .Where(p => p.UsuarioId == usuarioLogueado.Id)
                    .ToList();
            }
        }

        // Crear el ViewModel
        var trailerEmbedUrl = pelicula.TrailerUrl?.Replace("watch?v=", "embed/");
        var viewModel = new ComprarEntradaViewModel
        {
            PeliculaId = peliculaId,
            Titulo = pelicula.Titulo,
            Descripcion = pelicula.Descripcion,
            ImagenUrl = pelicula.ImagenUrl,
            TrailerUrl = trailerEmbedUrl,
            Capacidad = pelicula.Sala.Capacidad,
            Horarios = horarios,
            Promociones = promocionesUsuario // Vacío si el usuario es temporal o no autenticado
        };

        return View(viewModel);
    }
    [HttpPost]
    [Route("Entradas/Comprar/{peliculaId:int}")]
    public async Task<IActionResult> Comprar(int peliculaId, int horarioId, int cantidadEntradas, string asientosSeleccionados, int? promocionId = null)
    {
        Console.WriteLine($"peliculaId: {peliculaId}, horarioId: {horarioId}, cantidadEntradas: {cantidadEntradas}, asientosSeleccionados: {asientosSeleccionados}");

        try
        {
            // Validar datos iniciales
            if (peliculaId <= 0 || horarioId <= 0 || cantidadEntradas <= 0 || string.IsNullOrEmpty(asientosSeleccionados))
            {
                return BadRequest("Datos incompletos o inválidos.");
            }

            // Validar que el horario existe y está relacionado con una sala válida
            var horario = await _context.Horarios.Include(h => h.Sala).FirstOrDefaultAsync(h => h.Id == horarioId);
            if (horario == null)
            {
                Console.WriteLine("El horario seleccionado no es válido.");
                return BadRequest("Horario no válido.");
            }

            // Verificar capacidad de la sala para el horario
            if (cantidadEntradas > horario.Sala.Capacidad)
            {
                return BadRequest($"La cantidad de entradas excede la capacidad disponible en la sala ({horario.Sala.Capacidad}).");
            }

            // Validar que los asientos seleccionados no estén ocupados para el horario específico
            var asientosOcupados = _context.Entradas
                .Where(e => e.HorarioId == horarioId)
                .Select(e => e.Asientos)
                .ToList();

            var asientos = asientosSeleccionados.Split(',').Where(a => !string.IsNullOrEmpty(a)).ToList();
            if (asientos.Any(asiento => asientosOcupados.Contains(asiento)))
            {
                return BadRequest("Uno o más asientos seleccionados ya están ocupados.");
            }

            // Validar que el número de asientos coincide con la cantidad de entradas
            if (asientos.Count != cantidadEntradas)
            {
                return BadRequest("El número de asientos seleccionados no coincide con la cantidad de entradas.");
            }

            int usuarioId;
            string correoElectronico;
            bool esTemporal = false;

            // Validar usuario autenticado o procesar usuario temporal
            if (User.Identity.IsAuthenticated)
            {
                var claimUsuarioId = User.FindFirst("UsuarioId");
                if (claimUsuarioId == null)
                {
                    Console.WriteLine("Error: UsuarioId no está presente en las claims del usuario autenticado.");
                    return BadRequest("Error de autenticación: Usuario no válido.");
                }

                usuarioId = int.Parse(claimUsuarioId.Value);
                var usuarioLogueado = await _context.Usuarios.FindAsync(usuarioId);
                if (usuarioLogueado == null)
                {
                    Console.WriteLine("Error: Usuario logueado no encontrado en la base de datos.");
                    return BadRequest("Usuario no válido.");
                }

                correoElectronico = usuarioLogueado.CorreoElectronico;
            }
            else
            {
                TempData["peliculaId"] = peliculaId;
                TempData["horarioId"] = horarioId;
                TempData["cantidadEntradas"] = cantidadEntradas;
                TempData["asientosSeleccionados"] = asientosSeleccionados;
                return RedirectToAction("UsuarioTemporal");
            }

            Console.WriteLine($"Usuario autenticado con UsuarioId: {usuarioId}");

            decimal? descuentoAplicado = null;

            // Validar promoción, si corresponde
            if (promocionId.HasValue)
            {
                var promocion = await _context.Promociones.FindAsync(promocionId.Value);
                if (promocion == null || promocion.UsuarioId != usuarioId)
                {
                    return BadRequest("Promoción no válida o no disponible para este usuario.");
                }

                descuentoAplicado = promocion.Descuento;
                _context.Promociones.Remove(promocion);
                await _context.SaveChangesAsync();
            }

            // Crear las entradas
            var token = Guid.NewGuid().ToString(); // Generar un token único
            foreach (var asiento in asientos)
            {
                var entrada = new Entrada
                {
                    UsuarioId = usuarioId,
                    PeliculaId = peliculaId,
                    HorarioId = horarioId,
                    CantidadEntradas = 1,
                    FechaCompra = DateTime.Now,
                    Asientos = asiento,
                    EsTemporal = esTemporal,
                    DescuentoAplicado = descuentoAplicado,
                    CorreoElectronico = correoElectronico,
                    Token = token // Asignar el token generado

                };

                _context.Entradas.Add(entrada);
            }

            await _context.SaveChangesAsync();

            // Preparar y enviar el correo con QR
            var pelicula = await _context.Peliculas.FindAsync(peliculaId);
            var sala = horario.Sala;

            var qrContent = JsonConvert.SerializeObject(new
            {
                Pelicula = pelicula.Titulo,
                Horario = horario.FechaHora.ToString("g"),
                Asientos = asientosSeleccionados,
                FechaCompra = DateTime.Now.ToString("dd-MM-yyyy"),
                Sala = sala.NombreSala,
                Token = token // Agregar el token único

            });

            byte[] qrCodeImage = GenerateQrCode(token);

            string correoHtml = $@"
            <h1>¡Gracias por tu compra!</h1>
            <p><strong>Película:</strong> {pelicula.Titulo}</p>
            <p><strong>Horario:</strong> {horario.FechaHora}</p>
            <p><strong>Asientos:</strong> {asientosSeleccionados}</p>
            <p><strong>Sala:</strong> {sala.NombreSala}</p>
            <p>Adjunto encontrarás un código QR con los detalles de tu compra.</p>
            <img src='{pelicula.ImagenUrl}' alt='Imagen de la película' style='width:300px;' />
        ";

            await SendEmailWithQrAsync(correoElectronico, "Confirmación de tu compra", correoHtml, qrCodeImage);

            return RedirectToAction("Confirmacion", new { usuarioId = usuarioId });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al procesar la compra: {ex.Message}");
            return BadRequest("Ocurrió un error al procesar la compra.");
        }
    }


    [HttpGet]
    [Route("Entradas/Confirmacion")]
    public async Task<IActionResult> Confirmacion(int usuarioId)
    {
        // Obtener todas las entradas del usuario para esta compra
        var entradas = await _context.Entradas
            .Include(e => e.Pelicula)
            .Include(e => e.Horario)
            .Where(e => e.UsuarioId == usuarioId)
            .ToListAsync();

        // Verificar si alguna entrada tiene un descuento aplicado
        var descuentoAplicado = entradas.FirstOrDefault(e => e.DescuentoAplicado.HasValue)?.DescuentoAplicado;

        // Pasar la información del descuento a la vista
        ViewData["MensajeDescuento"] = descuentoAplicado.HasValue
            ? $"Se ha aplicado un descuento del {descuentoAplicado.Value}% en esta compra."
            : "No se ha aplicado ningún descuento en esta compra.";

        return View(entradas);
    }

    [HttpGet]
    [Route("Entradas/MisEntradas")]
    public IActionResult MisEntradas()
    {
        try
        {
            // Obtener el ID del usuario autenticado
            int usuarioId = int.Parse(User.FindFirst("UsuarioId")?.Value ?? "1");

            // Recuperar las entradas del usuario
            var entradas = _context.Entradas
                .Include(e => e.Pelicula)
                .Include(e => e.Horario)
                    .ThenInclude(h => h.Sala) // Incluimos la Sala asociada al Horario
                .Where(e => e.UsuarioId == usuarioId)

                .Select(e => new ComprarEntradaViewModel
                {
                    PeliculaId = e.PeliculaId,
                    Titulo = e.Pelicula.Titulo,
                    Descripcion = e.Pelicula.Descripcion,
                    ImagenUrl = e.Pelicula.ImagenUrl,
                    Horarios = new List<Horario> { e.Horario },
                    HorarioId = e.HorarioId,
                    CantidadEntradas = e.CantidadEntradas,
                    Capacidad = e.Horario.Sala.Capacidad, // Capacidad de la Sala
                    SalaNombre = e.Horario.Sala.NombreSala // Nombre de la Sala
                })
                .ToList();

            return View(entradas);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener las entradas: {ex.Message}");
            return BadRequest("Ocurrió un error al recuperar tus entradas.");
        }
    }

    [HttpGet]
    [Route("Entradas/AsientosOcupados/{peliculaId:int}/{horarioId:int}")]
    public IActionResult ObtenerAsientosOcupados(int peliculaId, int horarioId)
    {
        try
        {
            // Consulta todas las entradas asociadas a la película y horario
            var entradas = _context.Entradas
                .Where(e => e.PeliculaId == peliculaId && e.HorarioId == horarioId)
                .Select(e => e.Asientos) // Solo necesitamos la columna Asientos
                .ToList();

            // Extraer los asientos ocupados en formato "Fila:X Asiento:Y"
            var asientosOcupados = entradas
                .SelectMany(e => e.Split(';')) // Divide los asientos por ";"
                .Where(a => !string.IsNullOrEmpty(a)) // Filtra asientos vacíos
                .Distinct() // Evita duplicados
                .ToList();

            return Ok(asientosOcupados); // Retorna la lista de asientos ocupados al cliente
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener los asientos ocupados: {ex.Message}");
            return BadRequest("Error al procesar los asientos ocupados.");
        }
    }

    [HttpGet]
    [Route("Entradas/UsuarioTemporal")]
    public IActionResult UsuarioTemporal()
    {
        return View();
    }

    [HttpPost]
    [Route("Entradas/UsuarioTemporal")]
    public async Task<IActionResult> UsuarioTemporal(string nombre, string apellidos, string correoElectronico)
    {



        if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(apellidos) || string.IsNullOrEmpty(correoElectronico))
        {
            ModelState.AddModelError("", "El nombre, apellidos y correo electrónico son obligatorios.");
            return View();
        }

        try
        {
            // Crear un usuario temporal
            Random random = new Random();
            int usuarioId = random.Next(100, 201);

            var usuarioTemporal = new Usuario
            {
                Id = usuarioId,
                NombreUsuario = $"{nombre} {apellidos}",
                Contraseña = "temp",
                Rol = "Temporal",
                EsTemporal = true,
                CorreoElectronico = correoElectronico
            };

            _context.Usuarios.Add(usuarioTemporal);
            await _context.SaveChangesAsync();

            // Información de la compra desde TempData
            var peliculaId = (int)TempData["peliculaId"];
            var horarioId = (int)TempData["horarioId"];
            var cantidadEntradas = (int)TempData["cantidadEntradas"];
            var asientosSeleccionados = (string)TempData["asientosSeleccionados"];

            // Crear las entradas y generar el token
            var token = Guid.NewGuid().ToString(); // Generar un token único

            var asientos = asientosSeleccionados.Split(',').Where(a => !string.IsNullOrEmpty(a)).ToList();
            foreach (var asiento in asientos)
            {
                var entrada = new Entrada
                {
                    UsuarioId = usuarioTemporal.Id,
                    PeliculaId = peliculaId,
                    HorarioId = horarioId,
                    CantidadEntradas = 1,
                    FechaCompra = DateTime.Now,
                    Asientos = asiento,
                    EsTemporal = true,
                    CorreoElectronico = correoElectronico,
                    Token = token
                };

                _context.Entradas.Add(entrada);
            }

            await _context.SaveChangesAsync();

            // Obtener detalles de la película, horario y sala
            var pelicula = await _context.Peliculas.FindAsync(peliculaId);
            var horario = await _context.Horarios.FindAsync(horarioId);
            var sala = await _context.Salas.FindAsync(horario.SalaId);

            // Generar el QR solo con el token
            byte[] qrCodeImage = GenerateQrCode(token);

            // Contenido del correo
            string correoHtml = $@"
            <h1>¡Gracias por tu compra!</h1>
            <p><strong>Película:</strong> {pelicula.Titulo}</p>
            <p><strong>Horario:</strong> {horario.FechaHora.ToString("g")}</p>
            <p><strong>Asientos:</strong> {asientosSeleccionados}</p>
            <p><strong>Sala:</strong> {sala.NombreSala}</p>
            <p><strong>Fecha de Compra:</strong> {DateTime.Now.ToString("dd-MM-yyyy HH:mm")}</p>
            <p>Adjunto encontrarás un código QR. Al escanearlo, obtendrás un identificador único (token) para validar tu entrada.</p>
        ";

            // Enviar el correo
            await SendEmailWithQrAsync(correoElectronico, "Confirmación de tu compra", correoHtml, qrCodeImage);

            return RedirectToAction("Confirmacion", new { usuarioId = usuarioTemporal.Id });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al guardar el usuario temporal: {ex.Message}");
            return BadRequest("Ocurrió un error al guardar el usuario temporal.");
        }




    }

    public byte[] GenerateQrCode(string token, int pixelPerModule = 20)
    {
        using (var qrGenerator = new QRCodeGenerator())
        {
            var qrCodeData = qrGenerator.CreateQrCode(token, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new QRCode(qrCodeData);

            using (var bitmap = qrCode.GetGraphic(pixelPerModule))
            {
                using (var stream = new MemoryStream())
                {
                    bitmap.Save(stream, ImageFormat.Png);
                    return stream.ToArray();
                }
            }
        }
    }


    public async Task SendEmailWithQrAsync(string toEmail, string subject, string bodyHtml, byte[] qrCodeImage)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("CineApp", "tu_correo@cine.com"));
        message.To.Add(new MailboxAddress("", toEmail));
        message.Subject = subject;

        var builder = new BodyBuilder
        {
            HtmlBody = bodyHtml
        };

        builder.Attachments.Add("QR_Entrada.png", qrCodeImage, new ContentType("image", "png"));

        message.Body = builder.ToMessageBody();

        using (var client = new SmtpClient())
        {
            await client.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync("hilazoparra@gmail.com", "ulaf cdud plfm frsg");
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }

    public IActionResult ValidarToken(string token)
    {
        var entrada = _context.Entradas.FirstOrDefault(e => e.Token == token);
        if (entrada != null)
        {
            return Ok(new { mensaje = "Token válido", entrada });
        }
        return BadRequest("Token inválido o no encontrado.");
    }

    public async Task EnviarCorreo(string destinatario, string asunto, string cuerpo)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("CineApp", "tu_correo@gmail.com"));
        message.To.Add(new MailboxAddress("", destinatario));
        message.Subject = asunto;

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = cuerpo
        };

        message.Body = bodyBuilder.ToMessageBody();

        using (var client = new SmtpClient())
        {
            await client.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync("hilazoparra@gmail.com", "ulaf cdud plfm frsg");
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }


}

