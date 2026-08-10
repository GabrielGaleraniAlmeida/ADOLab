
# Integrantes

- Gabriel Frageri Dias - RM: 556830
- Gabriel Galerani - RM: 557421
- Gustavo Alves dos Santos Teixeira - RM: 557876
- Leo Taschin - RM: 554583


# ADOLab

Projeto de estudo de acesso a dados com ADO.NET, composto por uma biblioteca de domínio, uma aplicação console e uma aplicação web MVC.

## Desafio

- Implementar os métodos CRUD da classe `AlunoRepository`.

Os métodos devem permitir:

- Inserir alunos;
- Listar alunos;
- Atualizar alunos;
- Excluir alunos;
- Buscar alunos por propriedade e valor.

## Estrutura

```text
ADOLab/
├── ADOLab/              # Domínio e repository
├── ADOLab.Console/      # Aplicação console
├── ADOLab.Web/          # Aplicação web MVC
└── ADOLab.sln           # Solution
```

## Tecnologias

- .NET 8 (biblioteca e console)
- ASP.NET Core MVC (.NET 10)
- ADO.NET
- SQL Server

## Execução

Configure a connection string `SqlServerConnection` no `appsettings.json` da aplicação que será executada e use:

```bash
dotnet build ADOLab.sln
dotnet run --project ADOLab.Console/ADOLab.Console.csproj
```

Para executar a aplicação web:

```bash
dotnet run --project ADOLab.Web/ADOLab.Web.csproj
```
