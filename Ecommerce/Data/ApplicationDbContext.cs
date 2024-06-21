using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Ecommerce.Models;
using Microsoft.AspNetCore.Identity;
using System.Reflection.Emit;

namespace Ecommerce.Data;


public class ApplicationDbContext
    : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<BillingAddress> BillingAddresses { get; set; } = null!;
    public DbSet<ShippingAddress> ShippingAddresses { get; set; } = null!;
    public DbSet<PaymentInfo> PaymentInfos { get; set; } = null!;
    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<OrderItem> OrderItems { get; set; } = null!;
    public DbSet<Cart> Carts { get; set; } = null!;
    public DbSet<CartItem> CartItems { get; set; } = null!;

    public DbSet<Rating> Ratings { get; set; } = null!;
    public DbSet<Image> Images { get; set; } = null!;
    public DbSet<ProductView> ProductViews { get; set; } = null!;


    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options
        ) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {

        base.OnModelCreating(builder);
        builder.Entity<Rating>().HasKey(rating => new { rating.ProductId, rating.UserId });
        builder.Entity<Image>().HasKey(image => new { image.Url, image.ProductId });
        // builder.Entity<Product>().Property(p=>p.CreatedAt).HasDefaultValue(DateTime.UtcNow);
    }

}