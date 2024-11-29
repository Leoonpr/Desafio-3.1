using Microsoft.EntityFrameworkCore;

namespace ConsultorioOdontologico;

public class ConsultorioContext:DbContext
{
  public DbSet<Paciente> Pacientes { get; set; }
  public DbSet<Consulta> Consultas { get; set; }
  public ConsultorioContext(DbContextOptions<ConsultorioContext> options) : base(options)
  {
  }
  protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Mapeamento das entidades
            modelBuilder.Entity<Paciente>()
                .ToTable("Pacientes");

            modelBuilder.Entity<Consulta>()
                .ToTable("Consultas")
                .HasOne(c => c.Paciente)
                .WithMany()
                .HasForeignKey(c => c.Paciente.CPF);

            modelBuilder.Entity<Consulta>()
                .HasIndex(c => new { c.Paciente.CPF, c.Data })
                .IsUnique()
                .HasFilter("Data > CURRENT_DATE"); 

            base.OnModelCreating(modelBuilder);
        }
}
