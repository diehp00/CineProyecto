using System;
using System.ComponentModel.DataAnnotations;

public class Horario
{
  public int Id { get; set; } // Primary Key

    [Required]
    [DataType(DataType.DateTime)]
    [CustomValidation(typeof(Horario), nameof(ValidarFechaHora))]
    public DateTime FechaHora { get; set; }

    public int SalaId { get; set; } // Foreign Key from Sala
    public Sala? Sala { get; set; }

    public ICollection<Entrada>? Entradas { get; set; }

    public static ValidationResult ValidarFechaHora(DateTime fechaHora, ValidationContext context)
    {
        if (fechaHora < DateTime.Now.AddDays(7))
        {
            return new ValidationResult("La fecha debe ser al menos dentro de una semana.");
        }

        if (fechaHora.Minute != 0)
        {
            return new ValidationResult("Solo se permiten horas en punto.");
        }

        return ValidationResult.Success!;
    }
}
