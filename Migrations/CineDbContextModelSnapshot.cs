﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Cine.Migrations
{
    [DbContext(typeof(CineDbContext))]
    partial class CineDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("Entrada", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Asientos")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("CantidadEntradas")
                        .HasColumnType("int");

                    b.Property<string>("CorreoElectronico")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<decimal?>("DescuentoAplicado")
                        .HasColumnType("decimal(65,30)");

                    b.Property<bool>("EsTemporal")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTime>("FechaCompra")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("HorarioId")
                        .HasColumnType("int");

                    b.Property<int>("PeliculaId")
                        .HasColumnType("int");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("UsuarioId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("HorarioId");

                    b.HasIndex("PeliculaId");

                    b.HasIndex("UsuarioId");

                    b.ToTable("Entradas");
                });

            modelBuilder.Entity("Horario", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("FechaHora")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("SalaId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("SalaId");

                    b.ToTable("Horarios");
                });

            modelBuilder.Entity("Pelicula", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Descripcion")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime>("FechaEstreno")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Genero")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("ImagenUrl")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("SalaId")
                        .HasColumnType("int");

                    b.Property<string>("Titulo")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("TrailerUrl")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<double>("Valoracion")
                        .HasColumnType("double");

                    b.HasKey("Id");

                    b.HasIndex("SalaId");

                    b.ToTable("Peliculas");
                });

            modelBuilder.Entity("Promocion", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Descripcion")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<decimal>("Descuento")
                        .HasColumnType("decimal(65,30)");

                    b.Property<DateTime>("FechaFin")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("FechaInicio")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("UsuarioId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Promociones");
                });

            modelBuilder.Entity("Sala", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("Capacidad")
                        .HasColumnType("int");

                    b.Property<string>("NombreSala")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Salas");
                });

            modelBuilder.Entity("Usuario", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Contraseña")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("CorreoElectronico")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<bool>("EsTemporal")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("NombreUsuario")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int?>("PromocionID")
                        .HasColumnType("int");

                    b.Property<string>("Rol")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("PromocionID")
                        .IsUnique();

                    b.ToTable("Usuarios");
                });

            modelBuilder.Entity("Entrada", b =>
                {
                    b.HasOne("Horario", "Horario")
                        .WithMany("Entradas")
                        .HasForeignKey("HorarioId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Pelicula", "Pelicula")
                        .WithMany("Entradas")
                        .HasForeignKey("PeliculaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Usuario", "Usuario")
                        .WithMany("Entradas")
                        .HasForeignKey("UsuarioId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Horario");

                    b.Navigation("Pelicula");

                    b.Navigation("Usuario");
                });

            modelBuilder.Entity("Horario", b =>
                {
                    b.HasOne("Sala", "Sala")
                        .WithMany("Horarios")
                        .HasForeignKey("SalaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Sala");
                });

            modelBuilder.Entity("Pelicula", b =>
                {
                    b.HasOne("Sala", "Sala")
                        .WithMany("Peliculas")
                        .HasForeignKey("SalaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Sala");
                });

            modelBuilder.Entity("Usuario", b =>
                {
                    b.HasOne("Promocion", "Promocion")
                        .WithOne("Usuario")
                        .HasForeignKey("Usuario", "PromocionID")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("Promocion");
                });

            modelBuilder.Entity("Horario", b =>
                {
                    b.Navigation("Entradas");
                });

            modelBuilder.Entity("Pelicula", b =>
                {
                    b.Navigation("Entradas");
                });

            modelBuilder.Entity("Promocion", b =>
                {
                    b.Navigation("Usuario");
                });

            modelBuilder.Entity("Sala", b =>
                {
                    b.Navigation("Horarios");

                    b.Navigation("Peliculas");
                });

            modelBuilder.Entity("Usuario", b =>
                {
                    b.Navigation("Entradas");
                });
#pragma warning restore 612, 618
        }
    }
}
