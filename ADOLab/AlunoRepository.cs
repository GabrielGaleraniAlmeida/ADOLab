using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;

namespace ADOLab
{
    /// <summary>
    /// Repositório de acesso a dados da entidade <see cref="Aluno"/> usando ADO.NET puro.
    /// </summary>
    public class AlunoRepository
    {
        private readonly string _connectionString;

        /// <summary>
        /// Colunas permitidas na busca dinâmica. Serve como "lista branca" para impedir
        /// SQL Injection, já que nomes de coluna não podem ser passados como parâmetro.
        /// </summary>
        private static readonly Dictionary<string, string> ColunasPermitidas =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "Id", "Id" },
                { "Nome", "Nome" },
                { "Idade", "Idade" },
                { "Email", "Email" },
                { "DataNascimento", "DataNascimento" }
            };

        public AlunoRepository(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException("A connection string não pode ser vazia.", nameof(connectionString));
            }

            _connectionString = connectionString;
        }

        // ------------------------------------------------------------------
        // CREATE
        // ------------------------------------------------------------------

        /// <summary>
        /// Insere um aluno e devolve o Id gerado pelo banco.
        /// </summary>
        public int Inserir(Aluno aluno)
        {
            if (aluno == null)
            {
                throw new ArgumentNullException(nameof(aluno));
            }

            const string sql = @"
                INSERT INTO Alunos (Nome, Idade, Email, DataNascimento)
                VALUES (@Nome, @Idade, @Email, @DataNascimento);
                SELECT CAST(SCOPE_IDENTITY() AS int);";

            using (SqlConnection conexao = new SqlConnection(_connectionString))
            using (SqlCommand comando = new SqlCommand(sql, conexao))
            {
                comando.Parameters.Add("@Nome", SqlDbType.NVarChar, 150).Value = aluno.Nome;
                comando.Parameters.Add("@Idade", SqlDbType.Int).Value = aluno.Idade;
                comando.Parameters.Add("@Email", SqlDbType.NVarChar, 150).Value = aluno.Email;
                comando.Parameters.Add("@DataNascimento", SqlDbType.Date).Value = aluno.DataNascimento;

                conexao.Open();

                object resultado = comando.ExecuteScalar();
                int novoId = Convert.ToInt32(resultado);

                aluno.Id = novoId;
                return novoId;
            }
        }

        // ------------------------------------------------------------------
        // READ
        // ------------------------------------------------------------------

        /// <summary>
        /// Retorna todos os alunos cadastrados, ordenados por nome.
        /// </summary>
        public List<Aluno> Listar()
        {
            const string sql = @"
                SELECT Id, Nome, Idade, Email, DataNascimento
                FROM Alunos
                ORDER BY Nome;";

            List<Aluno> alunos = new List<Aluno>();

            using (SqlConnection conexao = new SqlConnection(_connectionString))
            using (SqlCommand comando = new SqlCommand(sql, conexao))
            {
                conexao.Open();

                using (SqlDataReader leitor = comando.ExecuteReader())
                {
                    while (leitor.Read())
                    {
                        alunos.Add(MapearAluno(leitor));
                    }
                }
            }

            return alunos;
        }

        /// <summary>
        /// Retorna um aluno pelo Id, ou null se não existir.
        /// </summary>
        public Aluno ObterPorId(int id)
        {
            const string sql = @"
                SELECT Id, Nome, Idade, Email, DataNascimento
                FROM Alunos
                WHERE Id = @Id;";

            using (SqlConnection conexao = new SqlConnection(_connectionString))
            using (SqlCommand comando = new SqlCommand(sql, conexao))
            {
                comando.Parameters.Add("@Id", SqlDbType.Int).Value = id;

                conexao.Open();

                using (SqlDataReader leitor = comando.ExecuteReader(CommandBehavior.SingleRow))
                {
                    if (leitor.Read())
                    {
                        return MapearAluno(leitor);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Busca alunos por uma propriedade e um valor.
        /// Textos usam comparação parcial (LIKE); os demais tipos, igualdade exata.
        /// </summary>
        /// <param name="propriedade">Nome da propriedade: Id, Nome, Idade, Email ou DataNascimento.</param>
        /// <param name="valor">Valor procurado.</param>
        public List<Aluno> BuscarPor(string propriedade, object valor)
        {
            if (string.IsNullOrWhiteSpace(propriedade))
            {
                throw new ArgumentException("Informe a propriedade da busca.", nameof(propriedade));
            }

            string coluna;
            if (!ColunasPermitidas.TryGetValue(propriedade.Trim(), out coluna))
            {
                throw new ArgumentException(
                    $"Propriedade '{propriedade}' não é válida. Use: {string.Join(", ", ColunasPermitidas.Keys)}.",
                    nameof(propriedade));
            }

            // Colunas de texto aceitam busca parcial; as demais exigem valor exato.
            bool colunaTexto = coluna == "Nome" || coluna == "Email";

            string sql = colunaTexto
                ? $@"SELECT Id, Nome, Idade, Email, DataNascimento
                     FROM Alunos
                     WHERE {coluna} LIKE @Valor
                     ORDER BY Nome;"
                : $@"SELECT Id, Nome, Idade, Email, DataNascimento
                     FROM Alunos
                     WHERE {coluna} = @Valor
                     ORDER BY Nome;";

            List<Aluno> alunos = new List<Aluno>();

            using (SqlConnection conexao = new SqlConnection(_connectionString))
            using (SqlCommand comando = new SqlCommand(sql, conexao))
            {
                object valorParametro = valor ?? DBNull.Value;

                if (colunaTexto)
                {
                    valorParametro = "%" + Convert.ToString(valor) + "%";
                }

                comando.Parameters.AddWithValue("@Valor", valorParametro);

                conexao.Open();

                using (SqlDataReader leitor = comando.ExecuteReader())
                {
                    while (leitor.Read())
                    {
                        alunos.Add(MapearAluno(leitor));
                    }
                }
            }

            return alunos;
        }

        // ------------------------------------------------------------------
        // UPDATE
        // ------------------------------------------------------------------

        /// <summary>
        /// Atualiza os dados de um aluno. Retorna true se alguma linha foi alterada.
        /// </summary>
        public bool Atualizar(Aluno aluno)
        {
            if (aluno == null)
            {
                throw new ArgumentNullException(nameof(aluno));
            }

            if (aluno.Id <= 0)
            {
                throw new ArgumentException("O aluno precisa ter um Id válido para ser atualizado.", nameof(aluno));
            }

            const string sql = @"
                UPDATE Alunos
                SET Nome = @Nome,
                    Idade = @Idade,
                    Email = @Email,
                    DataNascimento = @DataNascimento
                WHERE Id = @Id;";

            using (SqlConnection conexao = new SqlConnection(_connectionString))
            using (SqlCommand comando = new SqlCommand(sql, conexao))
            {
                comando.Parameters.Add("@Nome", SqlDbType.NVarChar, 150).Value = aluno.Nome;
                comando.Parameters.Add("@Idade", SqlDbType.Int).Value = aluno.Idade;
                comando.Parameters.Add("@Email", SqlDbType.NVarChar, 150).Value = aluno.Email;
                comando.Parameters.Add("@DataNascimento", SqlDbType.Date).Value = aluno.DataNascimento;
                comando.Parameters.Add("@Id", SqlDbType.Int).Value = aluno.Id;

                conexao.Open();

                return comando.ExecuteNonQuery() > 0;
            }
        }

        // ------------------------------------------------------------------
        // DELETE
        // ------------------------------------------------------------------

        /// <summary>
        /// Exclui um aluno pelo Id. Retorna true se alguma linha foi removida.
        /// </summary>
        public bool Excluir(int id)
        {
            const string sql = "DELETE FROM Alunos WHERE Id = @Id;";

            using (SqlConnection conexao = new SqlConnection(_connectionString))
            using (SqlCommand comando = new SqlCommand(sql, conexao))
            {
                comando.Parameters.Add("@Id", SqlDbType.Int).Value = id;

                conexao.Open();

                return comando.ExecuteNonQuery() > 0;
            }
        }

        // ------------------------------------------------------------------
        // Auxiliar
        // ------------------------------------------------------------------

        /// <summary>
        /// Converte a linha atual do SqlDataReader em um objeto Aluno.
        /// </summary>
        private static Aluno MapearAluno(SqlDataReader leitor)
        {
            int id = leitor.GetInt32(leitor.GetOrdinal("Id"));
            int idade = leitor.GetInt32(leitor.GetOrdinal("Idade"));

            int indiceNome = leitor.GetOrdinal("Nome");
            string nome = leitor.IsDBNull(indiceNome) ? string.Empty : leitor.GetString(indiceNome);

            int indiceEmail = leitor.GetOrdinal("Email");
            string email = leitor.IsDBNull(indiceEmail) ? string.Empty : leitor.GetString(indiceEmail);

            DateTime dataNascimento = leitor.GetDateTime(leitor.GetOrdinal("DataNascimento"));

            return new Aluno(id, nome, idade, email, dataNascimento);
        }
    }
}
