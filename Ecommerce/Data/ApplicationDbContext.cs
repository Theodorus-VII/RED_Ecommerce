using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Ecommerce.Models;
using Microsoft.AspNetCore.Identity;

namespace Ecommerce.Data;


public class ApplicationDbContext
    : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;
    //public DbSet<Order> Orders { get; set; } = null!;
    //public DbSet<OrderItem> Order_Items { get; set; } = null!;

    public DbSet<Address> Addresses { get; set; }
    public DbSet<PaymentInfo> PaymentInfos { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    //public DbSet<Cart> Carts { get; set; } = null!;
    //public DbSet<CartItem> Cart_Items { get; set; } = null!;
    public DbSet<CartItem> CartItems { get; set; } = null!;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options
        ) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        // mysql specific stupidity
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            var tableName = entityType.GetTableName();
            var schema = entityType.GetSchema();

            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(string) && (property.GetMaxLength() >= 255 || property.GetMaxLength() == null))
                {
                    property.SetMaxLength(100);
                }
            }
        }
    }

}