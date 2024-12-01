using Microsoft.EntityFrameworkCore;

namespace ConsultorioOdontologico;

public class ConsultorioContext:DbContext
{
  public DbSet<Paciente> Pacientes { get; set; }
  public DbSet<Consulta> Consultas { get; set; }
  /*public ConsultorioContext(DbContextOptions<ConsultorioContext> options) : base(options)
  {*/
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
            optionsBuilder.UseNpgsql("Host=localhost;Database=ConsultorioDB;Username=postgres;Password=root");
    }
  protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Paciente>()
                .ToTable("Pacientes");

            modelBuilder.Entity<Consulta>()
                .ToTable("Consultas")
                .HasOne(c => c.Paciente)
                .WithMany()
                .HasForeignKey(c => c.PacienteId);

            base.OnModelCreating(modelBuilder);
        }
}
