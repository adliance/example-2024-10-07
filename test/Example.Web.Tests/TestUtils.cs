using Example.Web.Models.Database;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)] // to make it easier to debug
namespace Example.Web.Tests;

internal static class TestUtils
{
    public static WebApplicationFactory<Program> Init(this WebApplicationFactory<Program> factory)
    {
        factory = factory.WithWebHostBuilder(config =>
        {
            config.ConfigureAppConfiguration((_, configBuilder) =>
            {
                // we could use a different database for tests, but for simplicities sake we just use the same one as in development
                /*
                configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["DatabaseConnectionString"] = ""
                });
                configBuilder.AddEnvironmentVariables();
                */
            });
            config.ConfigureServices(services => { });
        });

        var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<Db>();
        db.Database.EnsureDeleted(); // we ALWAYS start each test with an empty database; while this is slow, it ensures an identical clean data set for each test.
        db.Database.Migrate();
        return factory;
    }
}
