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
    
    public DbSet<User> Users { get; set; }
    
    public DbSet<Order> Orders { get; set; }
    
    public DbSet<OrderItem> OrderItems { get; set; }
    
    public DbSet<Recipe> Recipes { get; set; }
    
    public DbSet<Ingredient> Ingredients { get; set; }
    
    public DbSet<IngredientConversion> IngredientConversions { get; set; }
    public DbSet<Supply> Supplies { get; set; }
    public DbSet<SupplyConversion> SupplyConversions { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductComponent> ProductComponents { get; set; }
    
    public DbSet<RecipeIngredient> RecipeIngredients { get; set; }
    public DbSet<StockMovement> StockMovements { get; set; }
    public DbSet<FinancialEntry> FinancialEntries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
            optionsBuilder.UseNpgsql(_configuration.GetConnectionString("Default"));

        optionsBuilder.UseSnakeCaseNamingConvention();
    }
}