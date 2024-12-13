using Microsoft.EntityFrameworkCore;

public class CineDbContext : DbContext


{


    public CineDbContext(DbContextOptions<CineDbContext> options) : base(options)
    {
    }

    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Pelicula> Peliculas { get; set; }
    public DbSet<Sala> Salas { get; set; }
    public DbSet<Horario> Horarios { get; set; }
    public DbSet<Entrada> Entradas { get; set; }
    public DbSet<Promocion> Promociones { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {// Relación entre Entrada y Usuario
        modelBuilder.Entity<Entrada>()
            .HasOne(e => e.Usuario)
            .WithMany(u => u.Entradas)
            .HasForeignKey(e => e.UsuarioId);

        // Relación entre Entrada y Pelicula
        modelBuilder.Entity<Entrada>()
            .HasOne(e => e.Pelicula)
            .WithMany(p => p.Entradas)
            .HasForeignKey(e => e.PeliculaId);

        // Relación entre Entrada y Horario
        modelBuilder.Entity<Entrada>()
            .HasOne(e => e.Horario)
            .WithMany(h => h.Entradas)
            .HasForeignKey(e => e.HorarioId);

        // Relación entre Pelicula y Sala
        modelBuilder.Entity<Pelicula>()
            .HasOne(p => p.Sala)
            .WithMany(s => s.Peliculas)
            .HasForeignKey(p => p.SalaId);

        modelBuilder.Entity<Horario>()
       .HasOne(h => h.Sala)
       .WithMany(s => s.Horarios)
       .HasForeignKey(h => h.SalaId);

        // Relación uno a uno entre Usuario y Promocion

        modelBuilder.Entity<Usuario>()
            .HasOne(u => u.Promocion) // Usuario tiene una promoción
            .WithOne(p => p.Usuario) // Promoción tiene un usuario
            .HasForeignKey<Usuario>(u => u.PromocionID) // Clave foránea en Usuario
            .OnDelete(DeleteBehavior.SetNull); // Si se elimina la promoción, Usuario queda sin promoción


    }




    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseMySql("server=localhost;database=cine_db;user=root;password=",
            new MySqlServerVersion(new Version(8, 0, 21)));
    }
}
