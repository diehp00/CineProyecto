using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class ComprarEntradaViewModel
{
  [Required(ErrorMessage = "El ID de la película es obligatorio.")]
  [Range(1, int.MaxValue, ErrorMessage = "El ID de la película debe ser un valor positivo.")]
  public int PeliculaId { get; set; }        // ID de la película

  [Required(ErrorMessage = "El título de la película es obligatorio.")]
  [StringLength(200, ErrorMessage = "El título no debe superar los 200 caracteres.")]
  public string Titulo { get; set; }         // Título de la película

  [Required(ErrorMessage = "La descripción es obligatoria.")]
  [StringLength(500, ErrorMessage = "La descripción no debe superar los 500 caracteres.")]
  public string Descripcion { get; set; }    // Descripción de la película

  [Required(ErrorMessage = "La URL de la imagen es obligatoria.")]
  [Url(ErrorMessage = "El formato de la URL de la imagen no es válido.")]
  public string ImagenUrl { get; set; }      // URL de la imagen de la película

  public string TrailerUrl { get; set; }      // URL de la imagen de la película


  [Range(0, 10, ErrorMessage = "La valoración debe estar entre 0 y 10.")]
  public decimal Valoracion { get; set; }    // Valoración de la película

  [Required(ErrorMessage = "Debe especificar al menos un horario disponible.")]
  public List<Horario> Horarios { get; set; } // Lista de horarios disponibles

  [Required(ErrorMessage = "Debe seleccionar un horario.")]
  [Range(1, int.MaxValue, ErrorMessage = "El ID del horario debe ser un valor positivo.")]
  public int HorarioId { get; set; }         // Horario seleccionado

  [Required]
  [Range(1, int.MaxValue, ErrorMessage = "La cantidad de entradas debe ser al menos 1.")]
  public int CantidadEntradas { get; set; }

  public bool EsTemporal { get; set; }       // Indica si el usuario es temporal

  [Required(ErrorMessage = "El nombre de la sala es obligatorio.")]
  [StringLength(100, ErrorMessage = "El nombre de la sala no debe superar los 100 caracteres.")]
  public string SalaNombre { get; set; }     // Nombre de la Sala

  [Required(ErrorMessage = "La capacidad de la sala es obligatoria.")]
  [Range(1, int.MaxValue, ErrorMessage = "La capacidad de la sala debe ser un valor positivo.")]
  public int Capacidad { get; set; }         // Capacidad de la sala

  public List<Promocion> Promociones { get; set; } // Promociones disponibles

  [Range(1, int.MaxValue, ErrorMessage = "La promoción seleccionada no es válida.")]
  public int? PromocionId { get; set; } // Promoción seleccionada por el usuario

  public string Asientos { get; set; } // Almacena los asientos en formato "Fila:Asiento,Fila:Asiento,..."

}