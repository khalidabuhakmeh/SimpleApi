using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace HelloFleet.Tests;

public static class WebApplicationFactoryExtensions {
    internal static string? GenerateLink(this WebApplicationFactory<Program> app, string name, object? values = null)
    {
        var generator = app.Services.GetRequiredService<LinkGenerator>();
        return generator.GetPathByName(name, values);
    }
}