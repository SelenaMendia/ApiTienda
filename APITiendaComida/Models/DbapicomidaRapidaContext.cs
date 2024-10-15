using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace APITiendaComida.Models;

public partial class DbapicomidaRapidaContext : DbContext
{
    public DbapicomidaRapidaContext()
    {
    }

    public DbapicomidaRapidaContext(DbContextOptions<DbapicomidaRapidaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Carrito> Carritos { get; set; }

    public virtual DbSet<Categoria> Categorias { get; set; }

    public virtual DbSet<DetalleCarrito> DetalleCarritos { get; set; }

    public virtual DbSet<Producto> Productos { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Carrito>(entity =>
        {
            entity.HasKey(e => e.CarritoId).HasName("PK__Carrito__778D586B10CCEC2D");

            entity.ToTable("Carrito");

            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PrecioTotal).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Usuario).WithMany(p => p.Carritos)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Carrito__Usuario__3F466844");
        });

        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.HasKey(e => e.CategoriaId).HasName("PK__Categori__F353C1E533F4EB8E");

            entity.Property(e => e.Nombre).HasMaxLength(100);
        });

        modelBuilder.Entity<DetalleCarrito>(entity =>
        {
            entity.HasKey(e => e.DetalleId).HasName("PK__DetalleC__6E19D6DA905057FB");

            entity.ToTable("DetalleCarrito");

            entity.Property(e => e.PrecioUnitario).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Carrito).WithMany(p => p.DetalleCarritos)
                .HasForeignKey(d => d.CarritoId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__DetalleCa__Carri__440B1D61");

            entity.HasOne(d => d.Producto).WithMany(p => p.DetalleCarritos)
                .HasForeignKey(d => d.ProductoId)
                .HasConstraintName("FK__DetalleCa__Produ__44FF419A");
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.ProductoId).HasName("PK__Producto__A430AEA341F9D6EB");

            entity.Property(e => e.Descripción).HasMaxLength(255);
            entity.Property(e => e.ImagenUrl)
                .HasMaxLength(255)
                .HasColumnName("ImagenURL");
            entity.Property(e => e.ImagenUrllocal)
                .HasMaxLength(255)
                .HasColumnName("ImagenURLlocal");
            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.Precio).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.oCategoria).WithMany(p => p.Productos)
                .HasForeignKey(d => d.CategoriaId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Productos__Categ__3C69FB99");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.UsuarioId).HasName("PK__Usuarios__2B3DE7B8297D958C");

            entity.HasIndex(e => e.Correo, "UQ__Usuarios__60695A19484D19AD").IsUnique();

            entity.Property(e => e.Contraseña).HasMaxLength(100);
            entity.Property(e => e.Correo).HasMaxLength(255);
            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.Rol).HasMaxLength(50);
            entity.Property(e => e.Telefono).HasMaxLength(100);
            entity.Property(e => e.Usuario1)
                .HasMaxLength(100)
                .HasColumnName("Usuario");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
