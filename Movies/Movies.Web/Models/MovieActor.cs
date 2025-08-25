using System.ComponentModel.DataAnnotations;

namespace Movies.Web.Models;

public class MovieActor
{
    public int MovieId { get; set; }
    public Movie Movie { get; set; } = null!;

    public int ActorId { get; set; }
    public Actor Actor { get; set; } = null!;

    [Required, StringLength(100)]
    public string Rol { get; set; } = null!;
}
