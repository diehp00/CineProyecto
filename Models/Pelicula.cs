using System;
using System.ComponentModel.DataAnnotations;

public class Pelicula
{
     public int Id { get; set; }
    public string Titulo { get; set; }
    public string Descripcion { get; set; }
    public string Genero { get; set; }
    public DateTime FechaEstreno { get; set; }
    public string ImagenUrl { get; set; }
    public string TrailerUrl { get; set; }
    public double Valoracion { get; set; }

    public int SalaId { get; set; }  // No usar [Required]
    public Sala? Sala { get; set; }  // Permitir null en Sala usando ?

        public ICollection<Entrada> Entradas { get; set; } = new List<Entrada>(); // Asegúrate de inicializar la colección

}
