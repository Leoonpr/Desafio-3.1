using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ConsultorioOdontologico
{
   class Agenda
    {
        private List<Paciente> pacientes = new List<Paciente>();
        private List<Consulta> consultas = new List<Consulta>();

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

            bool temConsultasFuturas = await _context.Consultas.AnyAsync(c => c.Paciente.CPF == cpf && c.Data >= DateTime.Today);
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

            bool jaAgendado = await _context.Consultas.AnyAsync(c => c.Paciente.CPF == cpf && c.Data >= DateTime.Today);
            if (jaAgendado)
                return StatusCode.ConsultaJaAgendada;

            var novaConsulta = new Consulta(paciente, data, horaInicio, horaFim);
            bool horarioConflitante = await _context.Consultas.AnyAsync(c => c.VerificarConflito(novaConsulta));
            if (horarioConflitante)
                return StatusCode.HorarioIndisponivel;

            await _context.Consultas.AddAsync(novaConsulta);
            await _context.SaveChangesAsync();

            return StatusCode.Sucesso;
        }

        public async Task<StatusCode>  CancelarConsulta(string cpf, DateTime data, TimeSpan horaInicio)
        {
 var consulta = await _context.Consultas
                .Include(c => c.Paciente)
                .FirstOrDefaultAsync(c => c.Paciente.CPF == cpf && c.Data == data && c.HoraInicio == horaInicio);

            if (consulta == null)
                return StatusCode.AgendamentoNaoEncontrado;

            if (consulta.Data < DateTime.Today || (consulta.Data == DateTime.Today && consulta.HoraInicio < DateTime.Now.TimeOfDay))
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
                consultasQuery = consultasQuery.Where(c => c.Data >= dataInicial.Value && c.Data <= dataFinal.Value);

            return await consultasQuery.ToListAsync();
        }

        public async Task<List<Consulta>> ListarConsultasFuturas(Paciente paciente)
        {
            return await _context.Consultas
                .Where(c => c.Paciente.CPF == paciente.CPF &&
                            (c.Data > DateTime.Today || (c.Data == DateTime.Today && c.HoraInicio > DateTime.Now.TimeOfDay)))
                .OrderBy(c => c.Data)
                .ThenBy(c => c.HoraInicio)
                .ToListAsync();
        }

        public async Task<List<Consulta>> ListarConsultasPorPeriodo(DateTime dataInicial, DateTime dataFinal)
        {
            return await _context.Consultas
                .Include(c => c.Paciente)
                .Where(c => c.Data >= dataInicial && c.Data <= dataFinal)
                .OrderBy(c => c.Data)
                .ThenBy(c => c.HoraInicio)
                .ToListAsync();
        }

        public async Task<List<Consulta>> ListarConsultas()
        {
            return await  _context.Consultas
                .OrderBy(c => c.Data)
                .ThenBy(c => c.HoraInicio)
                .ToListAsync();
        }


    }
}
