using System.Collections.Generic;

namespace Movies.Web.ViewModels
{
    public class ReportViewModel
    {
        public List<MovieGroup> MoviesGrouped { get; set; } = new();
        public List<ActorGroup> ActorsGrouped { get; set; } = new();
    }

    // Filmlere göre oyuncular
    public class MovieGroup
    {
        public string FilmAdi { get; set; } = null!;
        public string Tur { get; set; } = null!;
        public int Yil { get; set; }
        public List<ActorDto> Oyuncular { get; set; } = new();
    }

    // Oyunculara göre filmler
    public class ActorGroup
    {
        public string ActorAdiSoyadi { get; set; } = null!;
        public List<MovieDto> Filmler { get; set; } = new();
    }

    // Küçük DTO’lar
    public class ActorDto
    {
        public string AdSoyad { get; set; } = null!;
        public string Rol { get; set; } = null!;
    }

    public class MovieDto
    {
        public string Ad { get; set; } = null!;
        public string Tur { get; set; } = null!;
        public int Yil { get; set; }
        public string Rol { get; set; } = null!;
    }
}
