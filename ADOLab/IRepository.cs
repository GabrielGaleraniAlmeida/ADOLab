/// <summary>
/// Define o contrato para um reposit�rio.
/// </summary>
/// <typeparam name="T">O tipo de entidade gerenciada pelo reposit�rio.</typeparam>
public interface IRepository<T>
{
    /// <summary>
    /// Obt�m ou define a string de conex�o com o banco de dados.
    /// </summary>
    string ConnectionString { get; set; }

    /// <summary>
    /// Garante que o esquema do banco de dados exista.
    /// </summary>
    void GarantirEsquema();

    /// <summary>
    /// Insere uma nova entidade no banco de dados.
    /// </summary>
    /// <param name="nome">O nome da entidade.</param>
    /// <param name="idade">A idade da entidade.</param>
    /// <param name="email">O email da entidade.</param>
    /// <param name="dataNascimento">A data de nascimento da entidade.</param>
    /// <returns>O ID da entidade rec�m-inserida.</returns>
    int Inserir(string nome, int idade, string email, DateTime dataNascimento);

    /// <summary>
    /// Recupera todas as entidades do banco de dados.
    /// </summary>
    /// <returns>Uma lista de entidades.</returns>
    List<T> Listar();

    /// <summary>
    /// Atualiza uma entidade no banco de dados.
    /// </summary>
    /// <param name="id">O ID da entidade a ser atualizada.</param>
    /// <param name="nome">O novo nome da entidade.</param>
    /// <param name="idade">A nova idade da entidade.</param>
    /// <param name="email">O novo email da entidade.</param>
    /// <param name="dataNascimento">A nova data de nascimento da entidade.</param>
    /// <returns>O n�mero de linhas afetadas.</returns>
    int Atualizar(int id, string nome, int idade, string email, DateTime dataNascimento);

    /// <summary>
    /// Exclui uma entidade do banco de dados.
    /// </summary>
    /// <param name="id">O ID da entidade a ser exclu�da.</param>
    /// <returns>O n�mero de linhas afetadas.</returns>
    int Excluir(int id);

    /// <summary>
    /// Busca entidades no banco de dados com base em um termo e valor.
    /// </summary>
    /// <param name="propriedade">O propriedade a ser pesquisada (coluna).</param>
    /// <param name="valor">O valor a ser pesquisado.</param>
    /// <returns>Uma lista de entidades correspondentes.</returns>
    List<T> Buscar(string propriedade, object valor);
}
