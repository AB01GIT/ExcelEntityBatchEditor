namespace TagReplacer.Context;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TagReplacer.Entities;

public class ProductContext : DbContext
{
    public DbSet<Product> Products { get; set; }

    private readonly string _connectionString;

    public ProductContext()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddJsonFile("appsettings.Local.json", optional: true)
            .Build();

        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(_connectionString);
    }
}