using System;
using System.Collections.Generic;
using ADOLab;
using Microsoft.Extensions.Configuration;

// Este arquivo NÃO declara namespace de propósito: se o projeto usar
// "namespace ADOLab.Console", a palavra Console passa a apontar para o
// namespace em vez da classe System.Console e nada compila.
internal class Program
{
    private static AlunoRepository _repositorio;

    private static void Main()
    {
        IConfigurationRoot configuracao = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        string connectionString = configuracao.GetConnectionString("SqlServerConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            Console.WriteLine("Connection string 'SqlServerConnection' não encontrada no appsettings.json.");
            return;
        }

        _repositorio = new AlunoRepository(connectionString);

        bool executando = true;

        while (executando)
        {
            Console.WriteLine();
            Console.WriteLine("===== ADOLab - Cadastro de Alunos =====");
            Console.WriteLine("1 - Listar alunos");
            Console.WriteLine("2 - Inserir aluno");
            Console.WriteLine("3 - Atualizar aluno");
            Console.WriteLine("4 - Excluir aluno");
            Console.WriteLine("5 - Buscar por propriedade");
            Console.WriteLine("0 - Sair");
            Console.Write("Opção: ");

            string opcao = Console.ReadLine();

            try
            {
                switch (opcao)
                {
                    case "1":
                        Listar();
                        break;
                    case "2":
                        Inserir();
                        break;
                    case "3":
                        Atualizar();
                        break;
                    case "4":
                        Excluir();
                        break;
                    case "5":
                        Buscar();
                        break;
                    case "0":
                        executando = false;
                        break;
                    default:
                        Console.WriteLine("Opção inválida.");
                        break;
                }
            }
            catch (Exception excecao)
            {
                Console.WriteLine($"Erro: {excecao.Message}");
            }
        }
    }

    // ----------------------------------------------------------------------

    private static void Listar()
    {
        List<Aluno> alunos = _repositorio.Listar();

        if (alunos.Count == 0)
        {
            Console.WriteLine("Nenhum aluno cadastrado.");
            return;
        }

        Console.WriteLine();
        Console.WriteLine($"{"Id",-5}{"Nome",-30}{"Idade",-8}{"Email",-30}{"Nascimento",-12}");
        Console.WriteLine(new string('-', 85));

        foreach (Aluno aluno in alunos)
        {
            ImprimirLinha(aluno);
        }
    }

    private static void Inserir()
    {
        Console.Write("Nome: ");
        string nome = Console.ReadLine();

        Console.Write("Idade: ");
        int idade = LerInteiro();

        Console.Write("Email: ");
        string email = Console.ReadLine();

        Console.Write("Data de nascimento (dd/MM/yyyy): ");
        DateTime dataNascimento = LerData();

        Aluno novo = new Aluno(0, nome, idade, email, dataNascimento);
        int id = _repositorio.Inserir(novo);

        Console.WriteLine($"Aluno cadastrado com o Id {id}.");
    }

    private static void Atualizar()
    {
        Console.Write("Id do aluno: ");
        int id = LerInteiro();

        Aluno aluno = _repositorio.ObterPorId(id);

        if (aluno == null)
        {
            Console.WriteLine("Aluno não encontrado.");
            return;
        }

        Console.WriteLine("Deixe em branco para manter o valor atual.");

        Console.Write($"Nome ({aluno.Nome}): ");
        string nome = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(nome))
        {
            aluno.Nome = nome;
        }

        Console.Write($"Idade ({aluno.Idade}): ");
        string idade = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(idade))
        {
            aluno.Idade = int.Parse(idade);
        }

        Console.Write($"Email ({aluno.Email}): ");
        string email = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(email))
        {
            aluno.Email = email;
        }

        Console.Write($"Data de nascimento ({aluno.DataNascimento:dd/MM/yyyy}): ");
        string data = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(data))
        {
            aluno.DataNascimento = DateTime.ParseExact(data, "dd/MM/yyyy", null);
        }

        bool atualizou = _repositorio.Atualizar(aluno);
        Console.WriteLine(atualizou ? "Aluno atualizado." : "Nada foi alterado.");
    }

    private static void Excluir()
    {
        Console.Write("Id do aluno: ");
        int id = LerInteiro();

        bool excluiu = _repositorio.Excluir(id);
        Console.WriteLine(excluiu ? "Aluno excluído." : "Aluno não encontrado.");
    }

    private static void Buscar()
    {
        Console.Write("Propriedade (Id, Nome, Idade, Email, DataNascimento): ");
        string propriedade = Console.ReadLine();

        Console.Write("Valor: ");
        string valor = Console.ReadLine();

        object valorConvertido = valor;

        if (string.Equals(propriedade, "Id", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(propriedade, "Idade", StringComparison.OrdinalIgnoreCase))
        {
            valorConvertido = int.Parse(valor);
        }
        else if (string.Equals(propriedade, "DataNascimento", StringComparison.OrdinalIgnoreCase))
        {
            valorConvertido = DateTime.ParseExact(valor, "dd/MM/yyyy", null);
        }

        List<Aluno> encontrados = _repositorio.BuscarPor(propriedade, valorConvertido);

        if (encontrados.Count == 0)
        {
            Console.WriteLine("Nenhum aluno encontrado.");
            return;
        }

        Console.WriteLine();
        foreach (Aluno aluno in encontrados)
        {
            ImprimirLinha(aluno);
        }
    }

    // ----------------------------------------------------------------------

    private static void ImprimirLinha(Aluno aluno)
    {
        Console.WriteLine($"{aluno.Id,-5}{aluno.Nome,-30}{aluno.Idade,-8}{aluno.Email,-30}{aluno.DataNascimento,-12:dd/MM/yyyy}");
    }

    private static int LerInteiro()
    {
        int valor;
        while (!int.TryParse(Console.ReadLine(), out valor))
        {
            Console.Write("Valor inválido. Digite um número inteiro: ");
        }

        return valor;
    }

    private static DateTime LerData()
    {
        DateTime valor;
        while (!DateTime.TryParseExact(Console.ReadLine(), "dd/MM/yyyy", null,
                   System.Globalization.DateTimeStyles.None, out valor))
        {
            Console.Write("Data inválida. Use o formato dd/MM/yyyy: ");
        }

        return valor;
    }
}