using Ecommerce.Repositories;
using Ecommerce.Services;


var builder = WebApplication.CreateBuilder(args);

// Repositórios
builder.Services.AddTransient<ICategoriaRepository>(_ => 
    new CategoriaDatabaseRepository(
        builder.Configuration.GetConnectionString("default")));

builder.Services.AddTransient<IProdutoRepository>(_ =>
    new ProdutoDatabaseRepository(
        builder.Configuration.GetConnectionString("default")));

builder.Services.AddTransient<IAvaliacaoRepository>(_ =>
    new AvaliacaoDatabaseRepository(
        builder.Configuration.GetConnectionString("default")));
        
builder.Services.AddTransient<IClienteRepository>(_ =>
    new ClienteDatabaseRepository(
        builder.Configuration.GetConnectionString("default")));

builder.Services.AddSingleton<PixService>();

builder.Services.AddSession();
builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseStaticFiles();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
