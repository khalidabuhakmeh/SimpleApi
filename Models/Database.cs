using Microsoft.EntityFrameworkCore;

namespace HelloFleet.Models;

public class Database : DbContext
{
    public Database(DbContextOptions<Database> options)
        : base(options)
    {
    }

    public DbSet<Person> Persons { get; set; } = default!;
}

public class Person
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}