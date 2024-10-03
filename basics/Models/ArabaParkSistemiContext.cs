using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace basics.Models;

public partial class ArabaParkSistemiContext : DbContext
{
    public ArabaParkSistemiContext()
    {
    }

    public ArabaParkSistemiContext(DbContextOptions<ArabaParkSistemiContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Araba> Arabas { get; set; }

    public virtual DbSet<Bölge> Bölges { get; set; }

    public virtual DbSet<BölgeGiris> BölgeGirises { get; set; }

    public virtual DbSet<Otopark> Otoparks { get; set; }

    //    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
    // => optionsBuilder.UseSqlServer("Server=DESKTOP-2K60S2G\\SQLEXPRESS;Database=ArabaParkSistemi;Trusted_Connection=True;TrustServerCertificate=True;");
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            // Bu satırları testler sırasında kullanmamak için yazılır.
            
            optionsBuilder.UseSqlServer("Server=DESKTOP-2K60S2G\\SQLEXPRESS;Database=ArabaParkSistemi;Trusted_Connection=True;TrustServerCertificate=True;");
            // Testlerde ise options kullanılarak yapılandırma sağlanır ve .UseSqlServer() çalışmaz.
        }
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Araba>(entity =>
        {
            entity.ToTable("Araba");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ArabaPlaka)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Boyut)
                .HasMaxLength(10)
                .IsFixedLength();
        });

        modelBuilder.Entity<Bölge>(entity =>
        {
            entity.ToTable("Bölge");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.OtoparkId).HasColumnName("OtoparkID");

            entity.HasOne(d => d.Otopark).WithMany(p => p.Bölges)
                .HasForeignKey(d => d.OtoparkId)
                .HasConstraintName("FK_Bölge_Otopark");
        });

        modelBuilder.Entity<BölgeGiris>(entity =>
        {
            entity.ToTable("BölgeGiris");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ArabaId).HasColumnName("ArabaID");
            entity.Property(e => e.BölgeId).HasColumnName("BölgeID");
            entity.Property(e => e.CikisZamani).HasColumnType("datetime");
            entity.Property(e => e.GirisZamani).HasColumnType("datetime");

            entity.HasOne(d => d.Araba).WithMany(p => p.BölgeGirises)
                .HasForeignKey(d => d.ArabaId)
                .HasConstraintName("FK_BölgeGiris_Araba");

            entity.HasOne(d => d.Bölge).WithMany(p => p.BölgeGirises)
                .HasForeignKey(d => d.BölgeId)
                .HasConstraintName("FK_BölgeGiris_Bölge");
        });

        modelBuilder.Entity<Otopark>(entity =>
        {
            entity.ToTable("Otopark");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
