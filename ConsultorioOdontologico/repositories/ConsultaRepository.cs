using Microsoft.EntityFrameworkCore;

namespace ConsultorioOdontologico.repositories;

public class ConsultaRepository(ConsultorioContext context)
{
      private readonly ConsultorioContext _context = context;

   public async Task<List<Consulta>> GetAllAsync()
    {
        return await _context.Consultas.Include(c => c.Paciente).ToListAsync();
    }

        public async Task<List<Consulta>> GetByPacienteCpfAsync(string cpf)
    {
        return await _context.Consultas
            .Where(c => c.Paciente.CPF == cpf)
            .Include(c => c.Paciente)
            .ToListAsync();
    }

    public async Task AddAsync(Consulta consulta)
    {
        await _context.Consultas.AddAsync(consulta);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var consulta = await _context.Consultas.FindAsync(id);
        if (consulta != null)
        {
            _context.Consultas.Remove(consulta);
            await _context.SaveChangesAsync();
        }
    }



}
