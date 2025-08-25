using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;// RandevuContext hangi namespace içindeyse onu yaz


namespace RandevuWeb.Data.Models;

public partial class RandevuContext : DbContext
{
    public RandevuContext()
    {
    }

    public RandevuContext(DbContextOptions<RandevuContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Kisiler> Kisilers { get; set; }

    public virtual DbSet<KisilerinRolleri> KisilerinRolleris { get; set; }

    public virtual DbSet<RandevuTipleri> RandevuTipleris { get; set; }

    public virtual DbSet<Randevular> Randevulars { get; set; }

    public virtual DbSet<Roller> Rollers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Server=localhost,1899;Database=RandevuDB;User Id=sa;Password=Password123*;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Kisiler>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Kisiler__3214EC0712F9F41D");

            entity.ToTable("Kisiler");

            entity.Property(e => e.Email).HasMaxLength(500);
            entity.Property(e => e.Name).HasMaxLength(250);
            entity.Property(e => e.PhoneNumber).HasMaxLength(11);
            entity.Property(e => e.Surname).HasMaxLength(250);
        });

        modelBuilder.Entity<KisilerinRolleri>(entity =>
        {
            // HasNoKey() yerine HasKey() ile birincil anahtarı tanımlıyoruz
            entity
                .HasKey(e => new { e.UserId, e.RoleId })
                .HasName("PK_KisilerinRolleri"); // Birincil anahtar adı tanımlıyoruz

            entity.ToTable("KisilerinRolleri");

            entity.Property(e => e.RoleId).HasMaxLength(450);
            entity.Property(e => e.UserId).HasMaxLength(450);

            entity.HasOne(d => d.Role).WithMany()
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_KisilerinRolleri_Roller");

            entity.HasOne(d => d.User).WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_KisilerinRolleri_Kisiler");
        });

        modelBuilder.Entity<RandevuTipleri>(entity =>
        {
            entity.ToTable("RandevuTipleri");

            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.TypeName).HasMaxLength(100);
        });

        modelBuilder.Entity<Randevular>(entity =>
        {
            entity.ToTable("Randevular");

            entity.Property(e => e.CancellReason).HasMaxLength(500);
            entity.Property(e => e.DoctorId).HasMaxLength(450);
            entity.Property(e => e.EndHour).HasMaxLength(5);
            entity.Property(e => e.PatientId).HasMaxLength(450);
            entity.Property(e => e.StartHour).HasMaxLength(5);

            entity.HasOne(d => d.AppointmentType).WithMany(p => p.Randevulars)
                .HasForeignKey(d => d.AppointmentTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Randevular_RandevuTipleri");

            entity.HasOne(d => d.Doctor).WithMany(p => p.RandevularDoctors)
                .HasForeignKey(d => d.DoctorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Randevular_Kisiler1");

            entity.HasOne(d => d.Patient).WithMany(p => p.RandevularPatients)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Randevular_Kisiler");
        });

        modelBuilder.Entity<Roller>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Roller__3214EC07C0DF0B02");

            entity.ToTable("Roller");

            entity.Property(e => e.Name).HasMaxLength(256);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
