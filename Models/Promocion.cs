using System;
using System.ComponentModel.DataAnnotations;

public class Promocion
{
    public int Id { get; set; }

    [Required]
    public required string Descripcion { get; set; }



    [DataType(DataType.Date)]
    public DateTime FechaInicio { get; set; }

    [DataType(DataType.Date)]
    public DateTime FechaFin { get; set; }

    public int Descuento { get; set; }

    public int UsuarioId { get; set; }

    public Usuario? Usuario { get; set; }

}
