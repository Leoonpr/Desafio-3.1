using Microsoft.EntityFrameworkCore;

namespace ConsultorioOdontologico;

public class PacienteRepository(ConsultorioContext context)
{
    private readonly ConsultorioContext _context = context;

    public async Task<List<Paciente>> GetAllAsync()
    {
        return await _context.Pacientes.OrderBy(p => p.Nome).ToListAsync();
    }

    public async Task<Paciente> GetByCpfAsync(string cpf)
    {
#pragma warning disable CS8603 // Possible null reference return.

        return await _context.Pacientes.FirstOrDefaultAsync(p => p.CPF == cpf);
#pragma warning restore CS8603 // Possible null reference return.

    }

     public async Task DeleteAsync(string cpf)
    {
        var paciente = await GetByCpfAsync(cpf);
        if (paciente != null)
        {
            _context.Pacientes.Remove(paciente);
            await _context.SaveChangesAsync();
        }
    }

        public async Task AddAsync(Paciente paciente)
    {
        await _context.Pacientes.AddAsync(paciente);
        await _context.SaveChangesAsync();
    }


}
