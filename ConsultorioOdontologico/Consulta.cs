using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultorioOdontologico
{
    public class Consulta
    {
        public int Id { get; set; }
        public Paciente Paciente { get; private set; }
        public DateTime Data { get; private set; }
        public TimeSpan HoraInicio { get; private set; }
        public TimeSpan HoraFim { get; private set; }

        public Consulta(Paciente paciente, DateTime data, TimeSpan horaInicio, TimeSpan horaFim)
        {
            Paciente = paciente;
            Data = data;
            HoraInicio = horaInicio;
            HoraFim = horaFim;
        }

        public bool VerificarConflito(Consulta outraConsulta)
        {
            return Data == outraConsulta.Data &&
                   (HoraInicio < outraConsulta.HoraFim && HoraFim > outraConsulta.HoraInicio);
        }
    }

}
