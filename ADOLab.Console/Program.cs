using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;

namespace ADOLab;

class Program
{
    static async Task Main(string[] args)
    {
        #region Config
        // Carrega a connection string do appsettings.json
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        string connString = config.GetConnectionString("SqlServerConnection")
            ?? throw new InvalidOperationException("ConnectionStrings:SqlServerConnection não encontrada.");
        #endregion

        var logger = new FileLogger("log.txt");

        try
        {
            var alunoRepo = new AlunoRepository(connString);
            await logger.LogAsync("Iniciando aplicação e garantindo o esquema.");
            alunoRepo.GarantirEsquema(); // DDL: cria a tabela se não existir

            while (true)
            {
                Console.WriteLine("\n=== CRUD ADO.NET – Alunos ===");
                Console.WriteLine("1) Inserir");
                Console.WriteLine("2) Listar");
                Console.WriteLine("3) Editar");
                Console.WriteLine("4) Deletar");
                Console.WriteLine("5) Buscar");
                Console.WriteLine("0) Sair");
                Console.Write("Escolha: ");
                var opc = Console.ReadLine();

                if (opc == "0") break;

                switch (opc)
                {
                    case "1":
                        Console.Write("Nome: "); var nome = Console.ReadLine() ?? "";
                        Console.Write("Idade: "); var idadeStr = Console.ReadLine();
                        Console.Write("Email: "); var email = Console.ReadLine() ?? "";
                        Console.Write("Data de Nascimento (yyyy-MM-dd): "); var dataNascimentoStr = Console.ReadLine();

                        if (int.TryParse(idadeStr, out int idade) && DateTime.TryParse(dataNascimentoStr, out DateTime dataNascimento))
                        {
                            int id = alunoRepo.Inserir(nome, idade, email, dataNascimento);
                            Console.WriteLine($"✅ Inserido Id={id}");
                            await logger.LogAsync($"Inserido aluno com Id={id}, Nome={nome}, Idade={idade}, Email={email}, DataNascimento={dataNascimento:yyyy-MM-dd}.");
                        }
                        else
                        {
                            Console.WriteLine("Dados inválidos.");
                            await logger.LogWarningAsync("Falha ao inserir aluno devido a dados inválidos.");
                        }
                        break;

                    case "2":
                        var alunos = alunoRepo.Listar();
                        Console.WriteLine("== Lista de Alunos ==");
                        foreach (var a in alunos)
                            Console.WriteLine($"#{a.Id} {a.Nome} ({a.Idade}) - {a.Email} - {a.DataNascimento:yyyy-MM-dd}");
                        Console.WriteLine(alunos.Count == 0 ? "(vazio)" : "");
                        await logger.LogAsync("Listou todos os alunos.");
                        break;

                    case "3":
                        Console.Write("Id: "); var idEditStr = Console.ReadLine();
                        Console.Write("Novo Nome: "); var novoNome = Console.ReadLine() ?? "";
                        Console.Write("Nova Idade: "); var novaIdadeStr = Console.ReadLine();
                        Console.Write("Novo Email: "); var novoEmail = Console.ReadLine() ?? "";
                        Console.Write("Nova Data de Nascimento (yyyy-MM-dd): "); var novaDataNascimentoStr = Console.ReadLine();

                        if (int.TryParse(idEditStr, out int idEdit) && int.TryParse(novaIdadeStr, out int novaIdade) && DateTime.TryParse(novaDataNascimentoStr, out DateTime novaDataNascimento))
                        {
                            int rows = alunoRepo.Atualizar(idEdit, novoNome, novaIdade, novoEmail, novaDataNascimento);
                            Console.WriteLine(rows > 0 ? "✅ Atualizado." : "⚠️ Nenhum registro afetado.");
                            await logger.LogAsync(rows > 0
                                ? $"Atualizado aluno Id={idEdit} com Nome={novoNome}, Idade={novaIdade}, Email={novoEmail}, DataNascimento={novaDataNascimento:yyyy-MM-dd}."
                                : $"Nenhum registro atualizado para Id={idEdit}.");
                        }
                        else
                        {
                            Console.WriteLine("Dados inválidos.");
                            await logger.LogWarningAsync("Falha ao atualizar aluno devido a dados inválidos.");
                        }
                        break;

                    case "4":
                        Console.Write("Id: "); var idDelStr = Console.ReadLine();
                        if (int.TryParse(idDelStr, out int idDel))
                        {
                            int rows = alunoRepo.Excluir(idDel);
                            Console.WriteLine(rows > 0 ? "✅ Deletado." : "⚠️ Nenhum registro afetado.");
                            await logger.LogAsync(rows > 0
                                ? $"Deletado aluno com Id={idDel}."
                                : $"Nenhum registro deletado para Id={idDel}.");
                        }
                        else
                        {
                            Console.WriteLine("Id inválido.");
                            await logger.LogWarningAsync("Falha ao deletar aluno devido a Id inválido.");
                        }
                        break;

                    case "5":
                        Console.Write("Propriedade (coluna): "); var propriedade = Console.ReadLine() ?? "";
                        Console.Write("Valor: "); var valor = Console.ReadLine() ?? "";
                        var resultados = alunoRepo.Buscar(propriedade, valor);
                        Console.WriteLine("== Resultados da Busca ==");
                        foreach (var r in resultados)
                            Console.WriteLine($"#{r.Id} {r.Nome} ({r.Idade}) - {r.Email} - {r.DataNascimento:yyyy-MM-dd}");
                        Console.WriteLine(resultados.Count == 0 ? "(vazio)" : "");
                        await logger.LogAsync($"Buscou pela propriedade '{propriedade}' com valor '{valor}'.");
                        break;

                    default:
                        Console.WriteLine("Opção inválida.");
                        await logger.LogWarningAsync("Opção de menu inválida selecionada.");
                        break;
                }
            }
        }
        catch (SqlException ex)
        {
            Console.WriteLine($"[ERRO SQL] {ex.Number} - {ex.Message}");
            await logger.LogErrorAsync($"Erro SQL {ex.Number}: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERRO] {ex.Message}");
            await logger.LogErrorAsync($"Exceção não tratada: {ex.Message}");
        }
    }
}
