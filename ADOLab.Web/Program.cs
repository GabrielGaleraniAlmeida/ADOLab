var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("SqlServerConnection")
    ?? throw new InvalidOperationException("ConnectionStrings:SqlServerConnection não encontrada.");

builder.Services.AddControllersWithViews();
builder.Services.AddScoped<AlunoRepository>(_ => new AlunoRepository(connectionString));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Alunos}/{action=Index}/{id?}");

app.Run();
