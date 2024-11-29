using System;

namespace ConsultorioOdontologico
{

    class Program
    {
        static void Main(string[] args)
        {
            Agenda agenda = new Agenda();
            bool executando = true;

            while (executando)
            {
                Console.Clear();
                Console.WriteLine("Menu Principal");
                Console.WriteLine("1 - Cadastro de pacientes");
                Console.WriteLine("2 - Agenda");
                Console.WriteLine("3 - Fim");
                Console.Write("Escolha uma opção: ");
                string opcaoPrincipal = Console.ReadLine();

                switch (opcaoPrincipal)
                {
                    case "1":
                        ExibirMenuCadastro(agenda);
                        break;
                    case "2":
                        ExibirMenuAgenda(agenda);
                        break;
                    case "3":
                        executando = false;
                        break;
                    default:
                        Console.WriteLine("Opção inválida. Pressione qualquer tecla para tentar novamente.");
                        Console.ReadKey();
                        break;
                }
            }
        }

        static void ExibirMenuCadastro(Agenda agenda)
        {
            bool voltarMenuPrincipal = false;

            while (!voltarMenuPrincipal)
            {
                Console.Clear();
                Console.WriteLine("Menu do Cadastro de Pacientes");
                Console.WriteLine("1 - Cadastrar novo paciente");
                Console.WriteLine("2 - Excluir paciente");
                Console.WriteLine("3 - Listar pacientes (ordenado por CPF)");
                Console.WriteLine("4 - Listar pacientes (ordenado por nome)");
                Console.WriteLine("5 - Voltar p/ menu principal");
                Console.Write("Escolha uma opção: ");
                string opcaoCadastro = Console.ReadLine();

                switch (opcaoCadastro)
                {
                    case "1":
                        CadastrarPaciente(agenda);
                        break;
                    case "2":
                        ExcluirPaciente(agenda);
                        break;
                    case "3":
                        ListarPacientes(agenda, ordenarPorNome: false);
                        break;
                    case "4":
                        ListarPacientes(agenda, ordenarPorNome: true);
                        break;
                    case "5":
                        voltarMenuPrincipal = true;
                        break;
                    default:
                        Console.WriteLine("Opção inválida. Pressione qualquer tecla para tentar novamente.");
                        Console.ReadKey();
                        break;
                }
            }
        }

        static void ExibirMenuAgenda(Agenda agenda)
        {
            bool voltarMenuPrincipal = false;

            while (!voltarMenuPrincipal)
            {
                Console.Clear();
                Console.WriteLine("Agenda");
                Console.WriteLine("1 - Agendar consulta");
                Console.WriteLine("2 - Cancelar agendamento");
                Console.WriteLine("3 - Listar agenda");
                Console.WriteLine("4 - Voltar p/ menu principal");
                Console.Write("Escolha uma opção: ");
                string opcaoAgenda = Console.ReadLine();

                switch (opcaoAgenda)
                {
                    case "1":
                        AgendarConsulta(agenda);
                        break;
                    case "2":
                        CancelarAgendamento(agenda);
                        break;
                    case "3":
                        ExibirAgenda(agenda);
                        break;
                    case "4":
                        voltarMenuPrincipal = true;
                        break;
                    default:
                        Console.WriteLine("Opção inválida. Pressione qualquer tecla para tentar novamente.");
                        Console.ReadKey();
                        break;
                }
            }
        }

        static void ExibirAgenda(Agenda agenda)
        {
            Console.Clear();
            Console.WriteLine("Apresentar a agenda T-Toda ou P-Periodo: ");
            string opcaoAgenda = Console.ReadLine().ToUpper();

            if (opcaoAgenda == "P")
            {
                Console.Write("Data inicial (DDMMAAAA): ");
                DateTime dataInicial = DateTime.ParseExact(Console.ReadLine(), "ddMMyyyy", null);

                Console.Write("Data final (DDMMAAAA): ");
                DateTime dataFinal = DateTime.ParseExact(Console.ReadLine(), "ddMMyyyy", null);

                var consultas = agenda.ListarConsultasPorPeriodo(dataInicial, dataFinal);
                ExibirConsultas(consultas);
            }
            else if (opcaoAgenda == "T")
            {
                var consultas = agenda.ListarConsultas();
                ExibirConsultas(consultas);
            }
            else
            {
                Console.WriteLine("Opção inválida.");
                Console.ReadKey();
            }
        }

        static void ExibirConsultas(List<Consulta> consultas)
        {
            Console.WriteLine("-------------------------------------------------------------");
            Console.WriteLine("Data       H.Ini  H.Fim   Tempo   Nome               Dt.Nasc.");
            Console.WriteLine("-------------------------------------------------------------");

            foreach (var consulta in consultas)
            {
                TimeSpan tempo = consulta.HoraFim - consulta.HoraInicio;
                Console.WriteLine($"{consulta.Data:dd/MM/yyyy} {consulta.HoraInicio:hh\\:mm} {consulta.HoraFim:hh\\:mm} {tempo:hh\\:mm} {consulta.Paciente.Nome,-20} {consulta.Paciente.DataNascimento:dd/MM/yyyy}");
            }

            Console.WriteLine("-------------------------------------------------------------");
            Console.WriteLine("Pressione qualquer tecla para continuar.");
            Console.ReadKey();
        }

    
        private static void CancelarAgendamento(Agenda agenda)
        {
            Console.Clear();
            Console.Write("CPF do paciente: ");
            string cpf = Console.ReadLine();
            Console.Write("Data da consulta a ser cancelada (DDMMAAAA): ");
            string dataConsultaInput = Console.ReadLine();
            Console.Write("Hora inicial da consulta (HHMM): ");
            string horaInicioInput = Console.ReadLine();

            if (DateTime.TryParseExact(dataConsultaInput, "ddMMyyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataConsulta) &&
                TimeSpan.TryParseExact(horaInicioInput, "hhmm", null, out TimeSpan horaInicio))
            {
                var status = agenda.CancelarConsulta(cpf, dataConsulta, horaInicio);
                ExibirMensagemStatus(status);
            }
            else
            {
                Console.WriteLine("Data ou hora inválida.");
            }
            Console.WriteLine("Pressione qualquer tecla para continuar.");
            Console.ReadKey();
        }



        static void AgendarConsulta(Agenda agenda)
        {
            Console.Clear();
            Console.Write("CPF do paciente: ");
            string cpf = Console.ReadLine();
            Console.Write("Data da consulta (DDMMAAAA): ");
            string dataConsultaInput = Console.ReadLine();
            DateTime dataConsulta;

            if (DateTime.TryParseExact(dataConsultaInput, "ddMMyyyy", null, System.Globalization.DateTimeStyles.None, out dataConsulta))
            {
                Console.Write("Hora de início (HH:MM): ");
                TimeSpan horaInicio;
                if (TimeSpan.TryParse(Console.ReadLine(), out horaInicio))
                {
                    Console.Write("Hora de fim (HH:MM): ");
                    TimeSpan horaFim;
                    if (TimeSpan.TryParse(Console.ReadLine(), out horaFim))
                    {
                        var status = agenda.AgendarConsulta(cpf, dataConsulta, horaInicio, horaFim);
                        ExibirMensagemStatus(status);
                    }
                    else
                    {
                        Console.WriteLine("Hora de fim inválida.");
                    }
                }
                else
                {
                    Console.WriteLine("Hora de início inválida.");
                }
            }
            else
            {
                Console.WriteLine("Data da consulta inválida.");
            }
            Console.WriteLine("Pressione qualquer tecla para continuar.");
            Console.ReadKey();
        }


        static void CadastrarPaciente(Agenda agenda)
        {
            Console.Clear();
            Console.Write("CPF: ");
            string cpf = Console.ReadLine();
            Console.Write("Nome: ");
            string nome = Console.ReadLine();
            Console.Write("Data de nascimento (DDMMAAAA): ");
            string dataNascimentoInput = Console.ReadLine();
            DateTime dataNascimento;

            if (DateTime.TryParseExact(dataNascimentoInput, "ddMMyyyy", null, System.Globalization.DateTimeStyles.None, out dataNascimento))
            {
                var status = agenda.CadastrarPaciente(cpf, nome, dataNascimento);
                ExibirMensagemStatus(status);
            }
            else
            {
                Console.WriteLine("Data de nascimento inválida.");
            }
            Console.WriteLine("Pressione qualquer tecla para continuar.");
            Console.ReadKey();
        }

        static void ExcluirPaciente(Agenda agenda)
        {
            Console.Clear();
            Console.Write("CPF do paciente a ser excluído: ");
            string cpf = Console.ReadLine();
            var status = agenda.ExcluirPaciente(cpf);
            ExibirMensagemStatus(status);
            Console.WriteLine("Pressione qualquer tecla para continuar.");
            Console.ReadKey();
        }

        static void ListarPacientes(Agenda agenda, bool ordenarPorNome)
        {
            Console.Clear();
            var pacientes = ordenarPorNome ? agenda.ListarPacientesPorNome() : agenda.ListarPacientesPorCPF();

            Console.WriteLine("------------------------------------------------------------");
            Console.WriteLine("CPF        Nome                        Dt.Nasc.  Idade");
            Console.WriteLine("------------------------------------------------------------");

            foreach (var paciente in pacientes)
            {
                Console.WriteLine($"{paciente.CPF} {paciente.Nome,-30} {paciente.DataNascimento:dd/MM/yyyy} {paciente.CalcularIdade()}");

                var consultasFuturas = agenda.ListarConsultasFuturas(paciente);
                if (consultasFuturas.Any())
                {
                    Console.WriteLine("  Consultas Futuras:");
                    foreach (var consulta in consultasFuturas)
                    {
                        Console.WriteLine($"    {consulta.Data:dd/MM/yyyy} {consulta.HoraInicio} - {consulta.HoraFim}");
                    }
                }
            }
            Console.WriteLine("------------------------------------------------------------");
            Console.WriteLine("Pressione qualquer tecla para continuar.");
            Console.ReadKey();
        }


        static void ExibirMensagemStatus(StatusCode status)
        {
            switch (status)
            {
                case StatusCode.CPFJaCadastrado:
                    Console.WriteLine("Erro: CPF já cadastrado.");
                    break;
                case StatusCode.CPFInvalido:
                    Console.WriteLine("Erro: CPF inválido.");
                    break;
                case StatusCode.NomeInvalido:
                    Console.WriteLine("Erro: Nome inválido. Deve ter pelo menos 5 caracteres.");
                    break;
                case StatusCode.IdadeInvalida:
                    Console.WriteLine("Erro: Paciente deve ter pelo menos 13 anos.");
                    break;
                case StatusCode.PacienteNaoEncontrado:
                    Console.WriteLine("Erro: Paciente não cadastrado.");
                    break;
                case StatusCode.PacienteComConsultaFutura:
                    Console.WriteLine("Erro: Paciente está agendado para uma consulta futura.");
                    break;
                case StatusCode.Sucesso:
                    Console.WriteLine("Operação realizada com sucesso.");
                    break;
                case StatusCode.ForaDoHorarioFuncionamento:
                    Console.WriteLine("Erro: Fora do horário de funcionamento.");
                    break;
                case StatusCode.HorarioIncorreto:
                    Console.WriteLine("Erro: Horário incorreto.");
                    break;
                case StatusCode.AgendamentoNaoEncontrado:
                    Console.WriteLine("Erro: Agendamento não encontrado.");
                    break;
                case StatusCode.HorarioIndisponivel:
                    Console.WriteLine("Erro: Já existe uma consulta agendada nesse horário");
                    break;
                case StatusCode.ConsultaJaAgendada:
                    Console.WriteLine("Erro: Consulta já agendada.");
                    break;
                default:
                    Console.WriteLine("Erro desconhecido.");
                    break;
            }
        }

    
    }
}