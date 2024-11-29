using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultorioOdontologico
{
    public enum StatusCode
    {
        Sucesso,
        CPFInvalido,
        NomeInvalido,
        IdadeInvalida,
        CPFJaCadastrado,
        PacienteNaoEncontrado,
        ConsultaJaAgendada,
        HorarioIndisponivel,
        AgendamentoNaoEncontrado,
        PacienteComConsultaFutura,
        ForaDoHorarioFuncionamento,
        HorarioInvalido,
        HorarioIncorreto
    }
}
