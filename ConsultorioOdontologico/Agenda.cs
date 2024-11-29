using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultorioOdontologico
{
   class Agenda
    {
        private List<Paciente> pacientes = new List<Paciente>();
        private List<Consulta> consultas = new List<Consulta>();

        public StatusCode CadastrarPaciente(string cpf, string nome, DateTime dataNascimento)
        {
            if (pacientes.Any(p => p.CPF == cpf))
                return StatusCode.CPFJaCadastrado;

            var paciente = new Paciente(cpf, nome, dataNascimento);
            var status = paciente.Validacao();
            if (status == StatusCode.Sucesso)
                pacientes.Add(paciente);
            return status;
        }

        public StatusCode ExcluirPaciente(string cpf)
        {
            var paciente = pacientes.FirstOrDefault(p => p.CPF == cpf);
            if (paciente == null)
                return StatusCode.PacienteNaoEncontrado;

            if (consultas.Any(c => c.Paciente.CPF == cpf && c.Data >= DateTime.Today))
                return StatusCode.PacienteComConsultaFutura;

            pacientes.Remove(paciente);
            consultas.RemoveAll(c => c.Paciente.CPF == cpf);
            return StatusCode.Sucesso;
        }

        public StatusCode AgendarConsulta(string cpf, DateTime data, TimeSpan horaInicio, TimeSpan horaFim)
        {
            var paciente = pacientes.FirstOrDefault(p => p.CPF == cpf);
            if (paciente == null)
                return StatusCode.PacienteNaoEncontrado;

            if (horaInicio >= horaFim || horaInicio.Minutes % 15 != 0 || horaFim.Minutes % 15 != 0)
                return StatusCode.HorarioIncorreto;

            if (horaInicio < new TimeSpan(8, 0, 0) || horaFim > new TimeSpan(19, 0, 0))
                return StatusCode.ForaDoHorarioFuncionamento;

            var agendamentoFuturo = consultas
             .Where(c => c.Paciente.CPF == cpf && c.Data >= DateTime.Today)
             .FirstOrDefault();

            if (agendamentoFuturo != null)
                return StatusCode.ConsultaJaAgendada;


            var novaConsulta = new Consulta(paciente, data, horaInicio, horaFim);
            if (consultas.Any(c => c.VerificarConflito(novaConsulta)))
                return StatusCode.HorarioIndisponivel;

            consultas.Add(novaConsulta);
            return StatusCode.Sucesso;
        }

        public StatusCode CancelarConsulta(string cpf, DateTime data, TimeSpan horaInicio)
        {
            var consulta = consultas.FirstOrDefault(c => c.Paciente.CPF == cpf && c.Data == data && c.HoraInicio == horaInicio);
            if (consulta == null)
                return StatusCode.AgendamentoNaoEncontrado;

            if (consulta.Data < DateTime.Today || (consulta.Data == DateTime.Today && consulta.HoraInicio < DateTime.Now.TimeOfDay))
                return StatusCode.HorarioIndisponivel;

            consultas.Remove(consulta);
            return StatusCode.Sucesso;
        }

        public List<Paciente> ListarPacientesPorNome() => pacientes.OrderBy(p => p.Nome).ToList();
        public List<Paciente> ListarPacientesPorCPF() => pacientes.OrderBy(p => p.CPF).ToList();

        public List<Consulta> ListarAgenda(DateTime? dataInicial = null, DateTime? dataFinal = null)
        {
            var listaConsultas = consultas.OrderBy(c => c.Data).ThenBy(c => c.HoraInicio).ToList();
            if (dataInicial.HasValue && dataFinal.HasValue)
                listaConsultas = listaConsultas
                    .Where(c => c.Data >= dataInicial.Value && c.Data <= dataFinal.Value)
                    .ToList();

            return listaConsultas;
        }

        public List<Consulta> ListarConsultasFuturas(Paciente paciente)
        {
            return consultas
                .Where(c => c.Paciente.CPF == paciente.CPF &&
                            (c.Data > DateTime.Today || (c.Data == DateTime.Today && c.HoraInicio > DateTime.Now.TimeOfDay)))
                .OrderBy(c => c.Data)
                .ThenBy(c => c.HoraInicio)
                .ToList();
        }

        public List<Consulta> ListarConsultasPorPeriodo(DateTime dataInicial, DateTime dataFinal)
        {
            return consultas
                .Where(c => c.Data >= dataInicial && c.Data <= dataFinal)
                .OrderBy(c => c.Data)
                .ThenBy(c => c.HoraInicio)
                .ToList();
        }

        public List<Consulta> ListarConsultas()
        {
            return consultas
                .OrderBy(c => c.Data)
                .ThenBy(c => c.HoraInicio)
                .ToList();
        }


    }
}
