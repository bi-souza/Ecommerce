using Ecommerce.Repositories;
using Ecommerce.Services;
using System.Globalization;
using Microsoft.AspNetCore.Localization;


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

var defaultCulture = new CultureInfo("pt-BR");
var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(defaultCulture),
    SupportedCultures = new List<CultureInfo> { defaultCulture },
    SupportedUICultures = new List<CultureInfo> { defaultCulture }
};
app.UseRequestLocalization(localizationOptions);

app.UseStaticFiles();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
