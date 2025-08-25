using Microsoft.EntityFrameworkCore;
using Movies.Web.Models;

namespace Movies.Web.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<Movie> Movies => Set<Movie>();
    public DbSet<Actor> Actors => Set<Actor>();
    public DbSet<MovieActor> MovieActors => Set<MovieActor>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MovieActor>()
            .HasKey(ma => new { ma.MovieId, ma.ActorId }); // composite key

        modelBuilder.Entity<MovieActor>()
            .HasOne(ma => ma.Movie)
            .WithMany(m => m.MovieActors)
            .HasForeignKey(ma => ma.MovieId);

        modelBuilder.Entity<MovieActor>()
            .HasOne(ma => ma.Actor)
            .WithMany(a => a.MovieActors)
            .HasForeignKey(ma => ma.ActorId);
    }

}
