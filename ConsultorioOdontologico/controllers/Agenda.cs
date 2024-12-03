using Microsoft.EntityFrameworkCore;

namespace ConsultorioOdontologico
{
    public class Agenda
    {
        private readonly ConsultorioContext _context;

        public Agenda(ConsultorioContext context)
        {
            _context = context;
        }

        public async Task<StatusCode> CadastrarPaciente(string cpf, string nome, DateTime dataNascimento)
        {
            if (await _context.Pacientes.AnyAsync(p => p.CPF == cpf))
                return StatusCode.CPFJaCadastrado;

            var paciente = new Paciente(cpf, nome, dataNascimento);
            var status = paciente.Validacao();

            if (status == StatusCode.Sucesso)
            {
                await _context.Pacientes.AddAsync(paciente);
                await _context.SaveChangesAsync();
            }

            return status;
        }

        public async Task<StatusCode> ExcluirPaciente(string cpf)
        {
            var paciente = await _context.Pacientes.FirstOrDefaultAsync(p => p.CPF == cpf);
            if (paciente == null)
                return StatusCode.PacienteNaoEncontrado;

            var hojeUtc = DateTime.UtcNow.Date;
            bool temConsultasFuturas = await _context.Consultas.AnyAsync(c => c.Paciente.CPF == cpf && c.Data >= hojeUtc);

            if (temConsultasFuturas)
                return StatusCode.PacienteComConsultaFutura;

            _context.Pacientes.Remove(paciente);

            var consultasDoPaciente = await _context.Consultas.Where(c => c.Paciente.CPF == cpf).ToListAsync();
            _context.Consultas.RemoveRange(consultasDoPaciente);

            await _context.SaveChangesAsync();
            return StatusCode.Sucesso;
        }

        public async Task<StatusCode> AgendarConsulta(string cpf, DateTime data, TimeSpan horaInicio, TimeSpan horaFim)
        {
            var paciente = await _context.Pacientes.FirstOrDefaultAsync(p => p.CPF == cpf);
            if (paciente == null)
                return StatusCode.PacienteNaoEncontrado;

            if (horaInicio >= horaFim || horaInicio.Minutes % 15 != 0 || horaFim.Minutes % 15 != 0)
                return StatusCode.HorarioIncorreto;

            if (horaInicio < new TimeSpan(8, 0, 0) || horaFim > new TimeSpan(19, 0, 0))
                return StatusCode.ForaDoHorarioFuncionamento;

            var consultasDoDia = await _context.Consultas
                .Where(c => c.Data == data)
                .ToListAsync();

            if (consultasDoDia.Any(c => c.HoraInicio < horaFim && c.HoraFim > horaInicio))
                return StatusCode.HorarioIndisponivel;

            var novaConsulta = new Consulta(paciente, data, horaInicio, horaFim);

            await _context.Consultas.AddAsync(novaConsulta);
            await _context.SaveChangesAsync();

            return StatusCode.Sucesso;
        }

        public async Task<StatusCode> CancelarConsulta(string cpf, DateTime data, TimeSpan horaInicio)
        {
            var consulta = await _context.Consultas
                .Include(c => c.Paciente)
                .FirstOrDefaultAsync(c => c.Paciente.CPF == cpf && c.Data == data && c.HoraInicio == horaInicio);

            if (consulta == null)
                return StatusCode.AgendamentoNaoEncontrado;

            var agoraUtc = DateTime.UtcNow;
            if (consulta.Data < agoraUtc.Date || (consulta.Data == agoraUtc.Date && consulta.HoraInicio < agoraUtc.TimeOfDay))
                return StatusCode.HorarioIndisponivel;

            _context.Consultas.Remove(consulta);
            await _context.SaveChangesAsync();

            return StatusCode.Sucesso;
        }

        public async Task<List<Paciente>> ListarPacientesPorNome()
        {
            return await _context.Pacientes.OrderBy(p => p.Nome).ToListAsync();
        }

        public async Task<List<Paciente>> ListarPacientesPorCPF()
        {
            return await _context.Pacientes.OrderBy(p => p.CPF).ToListAsync();
        }

        public async Task<List<Consulta>> ListarAgenda(DateTime? dataInicial = null, DateTime? dataFinal = null)
        {
            var consultasQuery = _context.Consultas
                .Include(c => c.Paciente)
                .OrderBy(c => c.Data)
                .ThenBy(c => c.HoraInicio)
                .AsQueryable();

            if (dataInicial.HasValue && dataFinal.HasValue)
            {
                var dataInicialUtc = dataInicial.Value.ToUniversalTime();
                var dataFinalUtc = dataFinal.Value.ToUniversalTime();
                consultasQuery = consultasQuery.Where(c => c.Data >= dataInicialUtc && c.Data <= dataFinalUtc);
            }

            return await consultasQuery.ToListAsync();
        }

        public async Task<List<Consulta>> ListarConsultasFuturas(Paciente paciente)
        {
            var agoraUtc = DateTime.UtcNow;
            return await _context.Consultas
                .Where(c => c.Paciente.CPF == paciente.CPF &&
                            (c.Data > agoraUtc.Date || (c.Data == agoraUtc.Date && c.HoraInicio > agoraUtc.TimeOfDay)))
                .OrderBy(c => c.Data)
                .ThenBy(c => c.HoraInicio)
                .ToListAsync();
        }

        public async Task<List<Consulta>> ListarConsultasPorPeriodo(DateTime dataInicial, DateTime dataFinal)
        {
            var dataInicialUtc = dataInicial.ToUniversalTime();
            var dataFinalUtc = dataFinal.ToUniversalTime();

            return await _context.Consultas
                .Include(c => c.Paciente)
                .Where(c => c.Data >= dataInicialUtc && c.Data <= dataFinalUtc)
                .OrderBy(c => c.Data)
                .ThenBy(c => c.HoraInicio)
                .ToListAsync();
        }

        public async Task<List<Consulta>> ListarConsultas()
        {
            return await _context.Consultas
                .OrderBy(c => c.Data)
                .ThenBy(c => c.HoraInicio)
                .ToListAsync();
        }
    }
}
