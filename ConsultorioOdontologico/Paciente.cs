using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultorioOdontologico
{
    public class Paciente
    {
        public int Id { get; set; }
        public string CPF { get; private set; }
        public string Nome { get; private set; }
        public DateTime DataNascimento { get; private set; }

        public Paciente(string cpf, string nome, DateTime dataNascimento)
        {
            CPF = cpf;
            Nome = nome;
            DataNascimento = dataNascimento;
        }

        public bool ValidarCpf()
        {
            string cpf = CPF.Replace(".", "").Replace("-", "").Trim();

            if (cpf.Length != 11 || cpf.Distinct().Count() == 1)
                return false;

            int soma = 0;
            for (int i = 0; i < 9; i++)
                soma += (cpf[i] - '0') * (10 - i);

            int primeiroDigitoVerificador = (soma * 10) % 11;
            if (primeiroDigitoVerificador == 10) primeiroDigitoVerificador = 0;
            if (primeiroDigitoVerificador != (cpf[9] - '0'))
                return false;

            soma = 0;
            for (int i = 0; i < 10; i++)
                soma += (cpf[i] - '0') * (11 - i);

            int segundoDigitoVerificador = (soma * 10) % 11;
            if (segundoDigitoVerificador == 10) segundoDigitoVerificador = 0;
            if (segundoDigitoVerificador != (cpf[10] - '0'))
                return false;

            return true;
        }


        public int CalcularIdade()
        {
            var dataAtual = DateTime.Today;
            int idade = dataAtual.Year - DataNascimento.Year;
            if (DataNascimento.Date > dataAtual.AddYears(-idade)) idade--;
            return idade;
        }

        public StatusCode Validacao()
        {
            if (!ValidarCpf())
                return StatusCode.CPFInvalido;

            if (Nome.Length < 5)
                return StatusCode.NomeInvalido;

            if (CalcularIdade() < 13)
                return StatusCode.IdadeInvalida;

            return StatusCode.Sucesso;
        }
    }
}
