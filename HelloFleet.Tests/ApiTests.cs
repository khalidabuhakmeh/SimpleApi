using System.Net.Http.Json;
using Xunit;
using HelloFleet.Models;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace HelloFleet.Tests;

public class ApiTests : ScenarioContext
{
    public ApiTests(WebAppFixture fixture)
        :base(fixture)
    {
    }

    [Fact]
    public async Task GetPersonList()
    {
        var client = Host.CreateClient();
        var path = Host.GenerateLink("person#index");
        var result = await client.GetAsync(path);

        Assert.Equal(Status200OK, (int)result.StatusCode);
    }

    [Fact]
    public async Task SavePerson()
    {
        var client = Host.CreateClient();
        var link = Host.GenerateLink("person#create");
        var result = await client.PostAsJsonAsync(link, new EditPersonRequest { Name= "Khalid"});

        var response = await result.Content.ReadFromJsonAsync<Person>();

        Assert.Equal(Status201Created, (int)result.StatusCode);
        Assert.Equal("Khalid", response?.Name);
    }

    [Fact]
    public async Task UpdatePerson()
    {
        var client = Host.CreateClient();
        var path = Host.GenerateLink("person#create");
        var post = await client.PostAsJsonAsync(path, new EditPersonRequest { Name= "Khalid"});
        var person = await post.Content.ReadFromJsonAsync<Person>();

        path = Host.GenerateLink("person#update", new { person!.Id });
        var result = await client.PutAsJsonAsync(path, new EditPersonRequest { Name = "Nicole" });
        var updated = await result.Content.ReadFromJsonAsync<Person>();

        Assert.Equal(Status200OK, (int)result.StatusCode);
        Assert.Equal("Nicole", updated?.Name);
    }

    [Fact]
    public async Task ShowPerson()
    {
        var client = Host.CreateClient();
        var path = Host.GenerateLink("person#create");
        var post = await client.PostAsJsonAsync(path, new EditPersonRequest { Name= "Maarten" });
        var person = await post.Content.ReadFromJsonAsync<Person>();

        path = Host.GenerateLink("person#show", new { person!.Id });
        var result = await client.GetFromJsonAsync<Person>(path);

        Assert.Equal("Maarten", result?.Name);
    }

    [Fact]
    public async Task DeletePerson()
    {
        var client = Host.CreateClient();
        var path = Host.GenerateLink("person#create");
        var post = await client.PostAsJsonAsync(path, new EditPersonRequest { Name= "Khalid"});
        var person = await post.Content.ReadFromJsonAsync<Person>();

        path = Host.GenerateLink("person#destroy", new { person!.Id });
        var result = await client.DeleteAsync(path);

        Assert.Equal(Status202Accepted, (int)result.StatusCode);
    }
}