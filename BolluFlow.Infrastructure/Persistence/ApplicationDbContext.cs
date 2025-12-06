using BolluFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BolluFlow.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    private readonly IConfiguration _configuration;
    
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options, 
        IConfiguration configuration) : base(options)
    {
        _configuration = configuration;
    }
    
    public DbSet<Customer> Customers { get; set; }
    
    public DbSet<DeliveryAddress> DeliveryAddresses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_configuration.GetConnectionString("Default"));
        optionsBuilder.UseSnakeCaseNamingConvention();
    }
}