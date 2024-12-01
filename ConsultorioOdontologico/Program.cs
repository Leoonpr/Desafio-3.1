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
                string opcao = Console.ReadLine();

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
                string opcao = Console.ReadLine();

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
                string opcao = Console.ReadLine();

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

        static async Task CadastrarPaciente(Agenda agenda)
        {
            Console.Clear();
            Console.WriteLine("===== CADASTRO DE PACIENTE =====");
            Console.Write("Nome: ");
            string nome = Console.ReadLine();
            Console.Write("CPF: ");
            string cpf = Console.ReadLine();
            Console.Write("Data de Nascimento (yyyy-MM-dd): ");
            DateTime dataNascimento = DateTime.Parse(Console.ReadLine());

            var status = await agenda.CadastrarPaciente(cpf, nome, dataNascimento);

            Console.WriteLine($"Resultado: {status}");
            Console.WriteLine("Pressione qualquer tecla para continuar.");
            Console.ReadKey();
        }

        static async Task ExcluirPaciente(Agenda agenda)
        {
            Console.Clear();
            Console.WriteLine("===== EXCLUSÃO DE PACIENTE =====");
            Console.Write("CPF do paciente a ser excluído: ");
            string cpf = Console.ReadLine();

            var status = await agenda.ExcluirPaciente(cpf);

            Console.WriteLine($"Resultado: {status}");
            Console.WriteLine("Pressione qualquer tecla para continuar.");
            Console.ReadKey();
        }

        static async Task ListarPacientes(Agenda agenda, bool ordenarPorNome)
        {
            Console.Clear();
            Console.WriteLine("===== LISTA DE PACIENTES =====");
            var pacientes = ordenarPorNome
                ? await agenda.ListarPacientesPorNome()
                : await agenda.ListarPacientesPorCPF();

            foreach (var paciente in pacientes)
            {
                Console.WriteLine($"Nome: {paciente.Nome}, CPF: {paciente.CPF}, Data de Nascimento: {paciente.DataNascimento:yyyy-MM-dd}");
            }

            Console.WriteLine("Pressione qualquer tecla para continuar.");
            Console.ReadKey();
        }

        static async Task AgendarConsulta(Agenda agenda)
        {
            Console.Clear();
            Console.WriteLine("===== AGENDAR CONSULTA =====");
            Console.Write("CPF do paciente: ");
            string cpf = Console.ReadLine();
            Console.Write("Data da consulta (yyyy-MM-dd): ");
            DateTime data = DateTime.Parse(Console.ReadLine());
            Console.Write("Hora de início (HH:mm): ");
            TimeSpan horaInicio = TimeSpan.Parse(Console.ReadLine());
            Console.Write("Hora de término (HH:mm): ");
            TimeSpan horaFim = TimeSpan.Parse(Console.ReadLine());

            var status = await agenda.AgendarConsulta(cpf, data, horaInicio, horaFim);

            Console.WriteLine($"Resultado: {status}");
            Console.WriteLine("Pressione qualquer tecla para continuar.");
            Console.ReadKey();
        }

        static async Task CancelarConsulta(Agenda agenda)
        {
            Console.Clear();
            Console.WriteLine("===== CANCELAR CONSULTA =====");
            Console.Write("CPF do paciente: ");
            string cpf = Console.ReadLine();
            Console.Write("Data da consulta (yyyy-MM-dd): ");
            DateTime data = DateTime.Parse(Console.ReadLine());
            Console.Write("Hora de início (HH:mm): ");
            TimeSpan horaInicio = TimeSpan.Parse(Console.ReadLine());

            var status = await agenda.CancelarConsulta(cpf, data, horaInicio);

            Console.WriteLine($"Resultado: {status}");
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
                Console.WriteLine($"Paciente: {consulta.Paciente.Nome}, Data: {consulta.Data:yyyy-MM-dd}, " +
                                  $"Hora: {consulta.HoraInicio:hh\\:mm} - {consulta.HoraFim:hh\\:mm}");
            }

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
