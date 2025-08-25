using System.ComponentModel.DataAnnotations;

namespace Movies.Web.Models;

public class Movie
{
    public int MovieId { get; set; }

    [Required, StringLength(200)]
    public string Ad { get; set; } = null!;     // film adı

    [Required, StringLength(100)]
    public string Tur { get; set; } = null!;    // tür/genre

    [Range(1900, 2100)]
    public int Yil { get; set; }

    public ICollection<MovieActor> MovieActors { get; set; } = new List<MovieActor>();
}
