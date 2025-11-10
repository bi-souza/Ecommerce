using Ecommerce.Repositories;
using Ecommerce.Services;


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

builder.Services.AddTransient<IClienteRepository>(_ =>
    new ClienteDatabaseRepository(
        builder.Configuration.GetConnectionString("default")));

builder.Services.AddTransient<IAdministradorRepository>(_ =>
    new AdministradorDatabaseRepository(
        builder.Configuration.GetConnectionString("default")));

builder.Services.AddTransient<IPedidoRepository>(_ =>
    new PedidoDatabaseRepository(
        builder.Configuration.GetConnectionString("default")));

builder.Services.AddSingleton<PixService>();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession();

builder.Services.AddControllersWithViews();

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

app.UseStaticFiles();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
