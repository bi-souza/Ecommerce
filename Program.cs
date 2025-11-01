
using Ecommerce.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<ICategoriaRepository>(_ => 
    new CategoriaDatabaseRepository(
        builder.Configuration.GetConnectionString("default")));

builder.Services.AddTransient<IProdutoRepository>(_ =>
    new ProdutoDatabaseRepository(
        builder.Configuration.GetConnectionString("default")));
        
builder.Services.AddTransient<IAvaliacaoRepository>(_ => 
    new AvaliacaoDatabaseRepository(
        builder.Configuration.GetConnectionString("default")));

builder.Services.AddSession();
builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseStaticFiles();

app.UseSession();

app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");

app.Run();
