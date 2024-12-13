using System.ComponentModel.DataAnnotations;

public class Usuario
{
    public int Id { get; set; }

    [Required]
    public required string NombreUsuario { get; set; }

    [Required]
    public required string Contraseña { get; set; }

    

    public string Rol { get; set; } = "Cliente";

    [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido.")]
    public required string CorreoElectronico { get; set; } 


    public int? PromocionID { get; set; }


    public bool EsTemporal { get; set; } // Indica si el usuario es temporal


    public ICollection<Entrada> Entradas { get; set; } = new List<Entrada>(); // Initialize to avoid null references

    public Promocion? Promocion { get; set; }

}
