using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class Sala
{
    public int Id { get; set; }
    public required string NombreSala { get; set; }
    public int Capacidad { get; set; }

    // Eliminar o modificar PeliculaId y Pelicula si Sala puede tener varias películas
    public ICollection<Pelicula> Peliculas { get; set; } = new List<Pelicula>();

    public required ICollection<Horario> Horarios { get; set; } = new List<Horario>();
}
