using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Ecommerce.Models;
using Microsoft.AspNetCore.Identity;
using Ecommerce.Models.ShoppingCart;

namespace Ecommerce.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Address> Addresses { get; set; } = null!;
        public DbSet<PaymentInfo> PaymentInfos { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<OrderItem> OrderItems { get; set; } = null!;
        public DbSet<CartItem> CartItems { get; set; } = null!;
        public DbSet<Cart> Carts { get; set; } = null!;
        public DbSet<Rating> Ratings { get; set; } = null!;
        public DbSet<Image> Images { get; set; } = null!;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure entities
            ConfigureProduct(builder);
            ConfigureCategory(builder);
            ConfigureAddress(builder);
            ConfigurePaymentInfo(builder);
            ConfigureOrder(builder);
            ConfigureOrderItem(builder);
            ConfigureCartItem(builder);
            ConfigureCart(builder);
            ConfigureRating(builder);
            ConfigureImage(builder);
        }

        private void ConfigureProduct(ModelBuilder builder)
        {
            builder.Entity<Product>().HasKey(p => p.Id);
            // Configure other properties and relationships
        }

        private void ConfigureCategory(ModelBuilder builder)
        {
            builder.Entity<Category>().HasKey(c => c.Id);
            // Configure other properties and relationships
        }

        private void ConfigureAddress(ModelBuilder builder)
        {
            builder.Entity<Address>().HasKey(a => a.AddressId);
            // Configure other properties and relationships
        }

        private void ConfigurePaymentInfo(ModelBuilder builder)
        {
            builder.Entity<PaymentInfo>().HasKey(p => p.PaymentInfoId);
            // Configure other properties and relationships
        }

        private void ConfigureOrder(ModelBuilder builder)
        {
            builder.Entity<Order>().HasKey(o => o.OrderId);
            // Configure other properties and relationships
        }

        private void ConfigureOrderItem(ModelBuilder builder)
        {
            builder.Entity<OrderItem>().HasKey(oi => oi.OrderItemId);
            // Configure other properties and relationships
        }

        private void ConfigureCartItem(ModelBuilder builder)
        {
            builder.Entity<CartItem>().HasKey(ci => ci.CartItemId);
            // Configure other properties and relationships
        }

        private void ConfigureCart(ModelBuilder builder)
        {
            builder.Entity<Cart>().HasKey(c => c.CartId);
            // Configure other properties and relationships
        }

        private void ConfigureRating(ModelBuilder builder)
        {
            builder.Entity<Rating>().HasKey(r => new { r.ProductId, r.UserId });
            // Configure other properties and relationships
        }

        private void ConfigureImage(ModelBuilder builder)
        {
            builder.Entity<Image>().HasKey(i => new { i.Url, i.ProductId });
            // Configure other properties and relationships
        }
    }
}
