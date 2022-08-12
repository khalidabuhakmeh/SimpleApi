using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace HelloFleet.Tests;

public class WebAppFixture : IAsyncLifetime
{
    internal SqliteConnection Connection = null!;
    internal WebApplicationFactory<Program> WebApp = null!;

    public Task InitializeAsync()
    {
        // keep this open and outside of DI
        Connection = new SqliteConnection("Filename=:memory:");
        Connection.Open();

        WebApp = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<Models.Database>));
                    services.Remove(descriptor!);
                    services.AddDbContext<Models.Database>(options =>
                    {
                        // into the void
                        options.UseLoggerFactory(NullLoggerFactory.Instance);
                        options.UseSqlite(Connection);
                    });

                    var sp = services.BuildServiceProvider();

                    using var scope = sp.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<Models.Database>();
                    db.Database.Migrate();
                });
            });
        
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        await WebApp.DisposeAsync();
        await Connection.DisposeAsync();
    }
}

[Collection("scenarios")]
public abstract class ScenarioContext
{
    protected ScenarioContext(WebAppFixture fixture)
    {
        Host = fixture.WebApp;
    }

    internal WebApplicationFactory<Program> Host { get; }
}

[CollectionDefinition("scenarios")]
public class ScenarioCollection : ICollectionFixture<WebAppFixture>
{

}