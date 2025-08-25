using System.ComponentModel.DataAnnotations;

namespace Movies.Web.Models;

public class Actor
{
    public int ActorId { get; set; }

    [Required, StringLength(100)]
    public string Ad { get; set; } = null!;

    [Required, StringLength(100)]
    public string Soyad { get; set; } = null!;

    public ICollection<MovieActor> MovieActors { get; set; } = new List<MovieActor>();
}
