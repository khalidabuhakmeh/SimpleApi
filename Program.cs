using FluentValidation;
using FluentValidation.AspNetCore;
using HelloFleet.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" }); });
builder.Services.AddDbContext<Database>(ob =>
{
    ob.LogTo(Console.WriteLine, LogLevel.Error)
        .UseSqlite("DataSource=database.db");
});
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

var app = builder.Build();

// Initialize Database
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<Database>();
    db.Database.Migrate();
}

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/", () =>
        Results.Redirect("/swagger"))
    .ExcludeFromDescription();

app.MapGet("/person", async (Database db) =>
        await db.Persons.ToListAsync())
    .WithName("person#index");
app.MapGet("/person/{id:int}", async (Database db, int id) =>
{
    var person = await db.Persons.FindAsync(id);
    return person is null ? Results.NotFound() : Results.Ok(person);
}).WithName("person#show");
app.MapPost("/person", async (EditPersonRequest request, Database db, IValidator<EditPersonRequest> validator) =>
{
    var validation = validator.Validate(request);
    if (validation.IsValid)
    {
        var result = db.Persons.Add(new Person { Name = request.Name });
        await db.SaveChangesAsync();
        return Results.CreatedAtRoute("person#show", new { id = result.Entity.Id }, result.Entity);
    }

    return Results.ValidationProblem(validation.ToDictionary());
});
app.MapPut("/person/{id:int}",
    async (Database db, int id, EditPersonRequest request, IValidator<EditPersonRequest> validator) =>
    {
        var entity = await db.Persons.FindAsync(id);
        if (entity is null)
            return Results.NotFound();

        var validation = validator.Validate(request);
        if (!validation.IsValid)
        {
            return Results.ValidationProblem(validation.ToDictionary());
        }

        entity.Name = request.Name!;
        return Results.Ok(entity);
    });
app.MapDelete("/person/{id:int}", async (Database db, int id) =>
{
    var entity = await db.Persons.FindAsync(id);
    if (entity is { })
    {
        db.Persons.Remove(entity);
        await db.SaveChangesAsync();
    }

    return Results.Accepted();
});

app.Run();