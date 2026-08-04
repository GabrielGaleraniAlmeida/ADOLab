using Microsoft.AspNetCore.Mvc;

namespace ADOLab.Web.Controllers;

public class AlunosController : Controller
{
    private readonly AlunoRepository _repository;

    public AlunosController(AlunoRepository repository)
    {
        _repository = repository;
    }

    public IActionResult Index()
    {
        var alunos = _repository.Listar();
        return View(alunos);
    }

    [HttpPost]
    public IActionResult Inserir([FromBody] AlunoInputModel? model)
    {
        try
        {
            if (model is null)
                return BadRequest(new { success = false, message = "Dados do aluno são obrigatórios." });

            var validationError = model.Validate();
            if (validationError is not null)
                return BadRequest(new { success = false, message = validationError });

            int id = _repository.Inserir(model.Nome, model.Idade, model.Email, model.DataNascimento);
            return Ok(new { success = true, id = id });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    public IActionResult Atualizar([FromBody] AlunoInputModel? model)
    {
        try
        {
            if (model is null)
                return BadRequest(new { success = false, message = "Dados do aluno são obrigatórios." });

            if (model.Id <= 0)
                return BadRequest(new { success = false, message = "ID inválido." });

            var validationError = model.Validate();
            if (validationError is not null)
                return BadRequest(new { success = false, message = validationError });

            int rows = _repository.Atualizar(model.Id, model.Nome, model.Idade, model.Email, model.DataNascimento);
            return Ok(new { success = rows > 0 });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    public IActionResult Deletar([FromBody] DeleteAlunoModel? model)
    {
        try
        {
            if (model is null || model.Id <= 0)
                return BadRequest(new { success = false, message = "ID inválido." });

            int rows = _repository.Excluir(model.Id);
            return Ok(new { success = rows > 0 });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    [HttpGet]
    public IActionResult ObterPorId(int id)
    {
        try
        {
            if (id <= 0)
                return BadRequest(new { success = false, message = "ID inválido." });

            var alunos = _repository.Listar();
            var aluno = alunos.FirstOrDefault(a => a.Id == id);

            if (aluno == null)
                return NotFound(new { success = false, message = "Aluno não encontrado." });

            return Ok(aluno);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }
}

public sealed class AlunoInputModel
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public int Idade { get; set; }
    public string Email { get; set; } = string.Empty;
    public DateTime DataNascimento { get; set; }

    public string? Validate()
    {
        if (string.IsNullOrWhiteSpace(Nome) || string.IsNullOrWhiteSpace(Email))
            return "Nome e Email são obrigatórios.";

        if (Idade < 0 || Idade > 120)
            return "A idade deve estar entre 0 e 120 anos.";

        if (DataNascimento == default)
            return "Data de nascimento é obrigatória.";

        try
        {
            if (!new System.Net.Mail.MailAddress(Email).Address.Equals(Email, StringComparison.OrdinalIgnoreCase))
                return "Email inválido.";
        }
        catch (FormatException)
        {
            return "Email inválido.";
        }

        return null;
    }
}

public sealed class DeleteAlunoModel
{
    public int Id { get; set; }
}
