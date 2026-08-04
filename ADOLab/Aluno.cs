/// <summary>
/// Representa uma entidade de estudante.
/// </summary>
public class Aluno
{
    /// <summary>
    /// Obt�m ou define o ID do estudante.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Obt�m ou define o nome do estudante.
    /// </summary>
    public string Nome { get; set; }

    /// <summary>
    /// Obt�m ou define a idade do estudante.
    /// </summary>
    public int Idade { get; set; }

    /// <summary>
    /// Obt�m ou define o email do estudante.
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// Obt�m ou define a data de nascimento do estudante.
    /// </summary>
    public DateTime DataNascimento { get; set; }

    /// <summary>
    /// Inicializa uma nova inst�ncia da classe <see cref="Aluno"/>.
    /// </summary>
    /// <param name="id">O ID do estudante.</param>
    /// <param name="nome">O nome do estudante.</param>
    /// <param name="idade">A idade do estudante.</param>
    /// <param name="email">O email do estudante.</param>
    /// <param name="dataNascimento">A data de nascimento do estudante.</param>
    public Aluno(int id, string nome, int idade, string email, DateTime dataNascimento)
    {
        Id = id;
        Nome = nome;
        Idade = idade;
        Email = email;
        DataNascimento = dataNascimento;
    }
}
