using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Ecommerce.Models;
using Microsoft.AspNetCore.Identity;
using Ecommerce.Models.ShoppingCart;

namespace Ecommerce.Data;


public class ApplicationDbContext
    : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<Address> Addresses { get; set; } = null!;
    public DbSet<PaymentInfo> PaymentInfos { get; set; } = null!;
    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<OrderItem> OrderItems { get; set; } = null!;
    public DbSet<Cart> Carts { get; set; } = null!;
    //public DbSet<CartItem> Cart_Items { get; set; } = null!;
    public DbSet<CartItem> CartItems { get; set; } = null!;

    //     public DbSet<Order> Orders { get; set; } = null!;
    //     public DbSet<Order_Item> Order_Items { get; set; } = null!;
    //     public DbSet<Basket> Baskets { get; set; } = null!;
    //     public DbSet<Basket_Item> Basket_Items { get; set; } = null!;
    public DbSet<Rating> Ratings { get; set; } = null!;
    public DbSet<Image> Images { get; set; } = null!;


    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options
        ) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<Rating>().HasKey(rating => new { rating.ProductId, rating.UserId });
        builder.Entity<Image>().HasKey(image => new { image.ProductId , image.Url});
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