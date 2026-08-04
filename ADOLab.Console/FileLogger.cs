using System.Text;

/// <summary>
/// Fornece funcionalidade para registrar mensagens em um arquivo de forma ass�ncrona.
/// </summary>
public class FileLogger
{
    private readonly string _filePath;

    /// <summary>
    /// Inicializa uma nova inst�ncia da classe <see cref="FileLogger"/>.
    /// </summary>
    /// <param name="filePath">O caminho para o arquivo de log.</param>
    public FileLogger(string filePath)
    {
        _filePath = filePath;
    }

    /// <summary>
    /// Registra uma mensagem informativa no arquivo de forma ass�ncrona.
    /// </summary>
    /// <param name="message">A mensagem a ser registrada.</param>
    public async Task LogAsync(string message)
    {
        await LogToFileAsync("INFO", message);
    }

    /// <summary>
    /// Registra uma mensagem de aviso no arquivo de forma ass�ncrona.
    /// </summary>
    /// <param name="message">A mensagem a ser registrada.</param>
    public async Task LogWarningAsync(string message)
    {
        await LogToFileAsync("WARNING", message);
    }

    /// <summary>
    /// Registra uma mensagem de erro no arquivo de forma ass�ncrona.
    /// </summary>
    /// <param name="message">A mensagem a ser registrada.</param>
    public async Task LogErrorAsync(string message)
    {
        await LogToFileAsync("ERROR", message);
    }

    /// <summary>
    /// Registra uma mensagem no arquivo com o tipo de log especificado de forma ass�ncrona.
    /// </summary>
    /// <param name="logType">O tipo de log (por exemplo, INFO, WARNING, ERROR).</param>
    /// <param name="message">A mensagem a ser registrada.</param>
    private async Task LogToFileAsync(string logType, string message)
    {
        try
        {
            string logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{logType}] {message}{Environment.NewLine}";
            await File.AppendAllTextAsync(_filePath, logMessage, Encoding.UTF8);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[LOG ERROR] {ex.Message}");
        }
    }
}
