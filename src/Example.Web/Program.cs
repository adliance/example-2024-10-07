using Example.Web.Factories.ViewModels.Home;
using Example.Web.Models.Database;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<Db>(options => { options.UseSqlServer(builder.Configuration["DatabaseConnectionString"]); });
builder.Services.AddTransient<IndexViewModelFactory>();
builder.Services.AddControllersWithViews();

var app = builder.Build();
app.UseHttpsRedirection();
app.UseDeveloperExceptionPage();
app.UseStaticFiles();
app.UseRouting();
app.MapDefaultControllerRoute();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<Db>();
    context.Database.Migrate();
}

app.Run();

// required to make it visible for integration tests
public partial class Program
{
}
