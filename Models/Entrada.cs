using System;
using System.ComponentModel.DataAnnotations;

public class Entrada
{
   public int Id { get; set; } // Primary Key

    [Required(ErrorMessage = "El ID de usuario es obligatorio.")]
    [Range(1, int.MaxValue, ErrorMessage = "El ID de usuario debe ser un valor positivo.")]
    public int UsuarioId { get; set; } // Foreign Key from Usuario

    [Required(ErrorMessage = "El ID de película es obligatorio.")]
    [Range(1, int.MaxValue, ErrorMessage = "El ID de película debe ser un valor positivo.")]
    public int PeliculaId { get; set; } // Foreign Key from Pelicula

    [Required(ErrorMessage = "El ID de horario es obligatorio.")]
    [Range(1, int.MaxValue, ErrorMessage = "El ID de horario debe ser un valor positivo.")]
    public int HorarioId { get; set; } // Foreign Key from Horario

    [Required(ErrorMessage = "La cantidad de entradas es obligatoria.")]
    [Range(1, 10, ErrorMessage = "La cantidad de entradas debe estar entre 1 y 10.")]
    public int CantidadEntradas { get; set; }

    [Required(ErrorMessage = "La fecha de compra es obligatoria.")]
    public DateTime FechaCompra { get; set; }

    public bool EsTemporal { get; set; } // Indica si la entrada es temporal

    [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
    [EmailAddress(ErrorMessage = "El correo electrónico no tiene un formato válido.")]
    public string CorreoElectronico { get; set; } // Campo requerido con validación de email

    [Required(ErrorMessage = "Es obligatorio proporcionar al menos un asiento.")]
    [RegularExpression(@"^(Fila:[A-Za-z0-9]+,?)*$", ErrorMessage = "El formato de los asientos no es válido.")]
    public string Asientos { get; set; } // Almacena los asientos en formato "Fila:Asiento,Fila:Asiento,..."

    public Usuario? Usuario { get; set; }
    public Pelicula? Pelicula { get; set; }
    public Horario? Horario { get; set; }

    [Range(0, 100, ErrorMessage = "El descuento aplicado debe estar entre 0% y 100%.")]
    public decimal? DescuentoAplicado { get; set; } // Validación opcional

        public string Token { get; set; } // Nuevo campo para el token





}
