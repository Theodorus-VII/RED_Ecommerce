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


    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options
        ) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {

        base.OnModelCreating(builder);
        builder.Entity<Rating>().HasKey(rating=>new {rating.ProductId,rating.UserId});
        builder.Entity<Image>().HasKey(image=>new {image.ProductId,image.Url});
        // builder.Entity<Product>().Property(p=>p.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        // mysql specific stupidity

        //builder.Entity<Order>()
        //.HasOne(o => o.BillingAddress)
        //.WithMany()
        //.HasForeignKey(o => o.BillingAddressId)
        //.OnDelete(DeleteBehavior.Restrict); // Adjust delete behavior as needed

        //builder.Entity<Order>()
        //    .HasOne(o => o.ShippingAddress)
        //    .WithMany()
        //    .HasForeignKey(o => o.ShippingAddressId)
        //    .OnDelete(DeleteBehavior.Restrict);
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