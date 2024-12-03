using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;

namespace ConsultorioOdontologico
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Configurando o host para injeção de dependência
            var host = CreateHostBuilder(args).Build();

            // Criando um escopo de serviços
            using var scope = host.Services.CreateScope();
            var agenda = scope.ServiceProvider.GetRequiredService<Agenda>();

            // Executando o menu principal
            await ExecutarMenuPrincipal(agenda);
        }

        static async Task ExecutarMenuPrincipal(Agenda agenda)
        {
            bool executando = true;

            while (executando)
            {
                Console.Clear();
                Console.WriteLine("===== MENU PRINCIPAL =====");
                Console.WriteLine("1 - Cadastro de pacientes");
                Console.WriteLine("2 - Agenda");
                Console.WriteLine("3 - Sair");
                Console.Write("Escolha uma opção: ");
                string? opcao = Console.ReadLine();

                switch (opcao)
                {
                    case "1":
                        await ExibirMenuCadastro(agenda);
                        break;
                    case "2":
                        await ExibirMenuAgenda(agenda);
                        break;
                    case "3":
                        executando = false;
                        Console.WriteLine("Saindo do sistema. Até logo!");
                        break;
                    default:
                        Console.WriteLine("Opção inválida! Pressione qualquer tecla para tentar novamente.");
                        Console.ReadKey();
                        break;
                }
            }
        }

        static async Task ExibirMenuCadastro(Agenda agenda)
        {
            bool voltar = false;

            while (!voltar)
            {
                Console.Clear();
                Console.WriteLine("===== MENU DE CADASTRO DE PACIENTES =====");
                Console.WriteLine("1 - Cadastrar novo paciente");
                Console.WriteLine("2 - Excluir paciente");
                Console.WriteLine("3 - Listar pacientes por CPF");
                Console.WriteLine("4 - Listar pacientes por Nome");
                Console.WriteLine("5 - Voltar ao menu principal");
                Console.Write("Escolha uma opção: ");
                string? opcao = Console.ReadLine();

                switch (opcao)
                {
                    case "1":
                        await CadastrarPaciente(agenda);
                        break;
                    case "2":
                        await ExcluirPaciente(agenda);
                        break;
                    case "3":
                        await ListarPacientes(agenda, ordenarPorNome: false);
                        break;
                    case "4":
                        await ListarPacientes(agenda, ordenarPorNome: true);
                        break;
                    case "5":
                        voltar = true;
                        break;
                    default:
                        Console.WriteLine("Opção inválida! Pressione qualquer tecla para tentar novamente.");
                        Console.ReadKey();
                        break;
                }
            }
        }

        static async Task CadastrarPaciente(Agenda agenda)
        {
            Console.Clear();
            Console.WriteLine("===== CADASTRO DE PACIENTE =====");
            Console.Write("Nome: ");
            string? nome = Console.ReadLine();
            Console.Write("CPF: ");
            string? cpf = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(nome) || string.IsNullOrWhiteSpace(cpf))
            {
                Console.WriteLine("Erro: Nome ou CPF inválidos.");
                Console.WriteLine("Pressione qualquer tecla para continuar.");
                Console.ReadKey();
                return;
            }
            Console.Write("Data de Nascimento (ddMMyyyy): ");
            DateTime dataNascimento = LerData();

            dataNascimento = DateTime.SpecifyKind(dataNascimento, DateTimeKind.Utc);

            var status = await agenda.CadastrarPaciente(cpf, nome, dataNascimento);
            ExibirMensagemStatus(status);
            Console.WriteLine("Pressione qualquer tecla para continuar.");
            Console.ReadKey();
        }

        static DateTime LerData()
        {
            while (true)
            {
                string? entrada = Console.ReadLine();
                if (DateTime.TryParseExact(entrada, "ddMMyyyy", null, System.Globalization.DateTimeStyles.None, out var data))
                {
                    return data;
                }
                Console.Write("Formato inválido! Digite novamente (ddMMyyyy): ");
            }
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

        static async Task ExibirMenuAgenda(Agenda agenda)
{
    bool voltar = false;

    while (!voltar)
    {
        Console.Clear();
        Console.WriteLine("===== MENU DE AGENDA =====");
        Console.WriteLine("1 - Agendar nova consulta");
        Console.WriteLine("2 - Cancelar consulta");
        Console.WriteLine("3 - Listar consultas");
        Console.WriteLine("4 - Voltar ao menu principal");
        Console.Write("Escolha uma opção: ");
        string? opcao = Console.ReadLine();

        switch (opcao)
        {
            case "1":
                await AgendarConsulta(agenda);
                break;
            case "2":
                await CancelarConsulta(agenda);
                break;
            case "3":
                await ListarConsultas(agenda);
                break;
            case "4":
                voltar = true;
                break;
            default:
                Console.WriteLine("Opção inválida! Pressione qualquer tecla para tentar novamente.");
                Console.ReadKey();
                break;
        }
    }
}

static async Task AgendarConsulta(Agenda agenda)
{
    Console.Clear();
    Console.WriteLine("===== AGENDAR CONSULTA =====");
    Console.Write("CPF do paciente: ");
    string? cpf = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(cpf))
    {
        Console.WriteLine("Erro: CPF não pode ser nulo.");
        Console.WriteLine("Pressione qualquer tecla para continuar.");
        Console.ReadKey();
        return;
    }
    Console.Write("Data da consulta (ddMMyyyy): ");
    DateTime data = LerData();
    Console.Write("Hora de início (HH:mm): ");
    TimeSpan horaInicio = LerHora();
    Console.Write("Hora de término (HH:mm): ");
    TimeSpan horaFim = LerHora();

    var status = await agenda.AgendarConsulta(cpf, DateTime.SpecifyKind(data, DateTimeKind.Utc), horaInicio, horaFim);
    ExibirMensagemStatus(status);
    Console.WriteLine("Pressione qualquer tecla para continuar.");
    Console.ReadKey();
}

static async Task CancelarConsulta(Agenda agenda)
{
    Console.Clear();
    Console.WriteLine("===== CANCELAR CONSULTA =====");
    Console.Write("CPF do paciente: ");
    string? cpf = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(cpf))
            {
                Console.WriteLine("Erro: CPF não pode ser nulo.");
                Console.WriteLine("Pressione qualquer tecla para continuar.");
                Console.ReadKey();
                return;
            }
    Console.Write("Data da consulta (ddMMyyyy): ");
    DateTime data = LerData();
    Console.Write("Hora de início (HH:mm): ");
    TimeSpan horaInicio = LerHora();

    var status = await agenda.CancelarConsulta(cpf, data, horaInicio);
    ExibirMensagemStatus(status);
    Console.WriteLine("Pressione qualquer tecla para continuar.");
    Console.ReadKey();
}

static async Task ListarConsultas(Agenda agenda)
{
    Console.Clear();
    Console.WriteLine("===== LISTA DE CONSULTAS =====");
    var consultas = await agenda.ListarConsultas();

    foreach (var consulta in consultas)
    {
        Console.WriteLine($"Paciente: {consulta.Paciente.Nome}, Data: {consulta.Data:dd/MM/yyyy}, " +
                          $"Hora: {consulta.HoraInicio:hh\\:mm} - {consulta.HoraFim:hh\\:mm}");
    }

    Console.WriteLine("Pressione qualquer tecla para continuar.");
    Console.ReadKey();
}

static TimeSpan LerHora()
{
    while (true)
    {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                string entrada = Console.ReadLine();
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                if (TimeSpan.TryParse(entrada, out var hora))
        {
            return hora;
        }
        Console.Write("Formato inválido! Digite novamente (HH:mm): ");
    }
}

static async Task ListarPacientes(Agenda agenda, bool ordenarPorNome)
{
    Console.Clear();
    Console.WriteLine("===== LISTA DE PACIENTES =====");
    var pacientes = ordenarPorNome
        ? await agenda.ListarPacientesPorNome()
        : await agenda.ListarPacientesPorCPF();

    if (pacientes.Count == 0)
    {
        Console.WriteLine("Nenhum paciente cadastrado.");
    }
    else
    {
        foreach (var paciente in pacientes)
        {
            Console.WriteLine($"Nome: {paciente.Nome}, CPF: {paciente.CPF}, Data de Nascimento: {paciente.DataNascimento:dd/MM/yyyy}");
        }
    }

    Console.WriteLine("Pressione qualquer tecla para continuar.");
    Console.ReadKey();
}

static async Task ExcluirPaciente(Agenda agenda)
{
    Console.Clear();
    Console.WriteLine("===== EXCLUSÃO DE PACIENTE =====");
    Console.Write("CPF do paciente a ser excluído: ");
    string? cpf = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(cpf))
    {
        Console.WriteLine("Erro: CPF não pode ser nulo.");
        Console.WriteLine("Pressione qualquer tecla para continuar.");
        Console.ReadKey();
        return;
    }
    var status = await agenda.ExcluirPaciente(cpf);
    ExibirMensagemStatus(status);

    Console.WriteLine("Pressione qualquer tecla para continuar.");
    Console.ReadKey();
}


        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((_, services) =>
                    services.AddDbContext<ConsultorioContext>()
                            .AddScoped<Agenda>());
    }
}
