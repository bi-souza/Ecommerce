using Ecommerce.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Repositórios
builder.Services.AddTransient<ICategoriaRepository>(_ => 
    new CategoriaDatabaseRepository(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddTransient<IProdutoRepository>(_ =>
    new ProdutoDatabaseRepository(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddTransient<IAvaliacaoRepository>(_ =>
    new AvaliacaoDatabaseRepository(
        builder.Configuration.GetConnectionString("DefaultConnection")));
        
builder.Services.AddTransient<IClienteRepository>(_ =>
    new ClienteDatabaseRepository(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Sessões e MVC
builder.Services.AddSession();
builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseStaticFiles();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
