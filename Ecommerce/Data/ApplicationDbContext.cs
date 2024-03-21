using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Ecommerce.Models;
using Microsoft.AspNetCore.Identity;

namespace Ecommerce.Data;

public class ApplicationDbContext
    : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Order_Item> Order_Items { get; set; }
    public DbSet<Basket> Baskets { get; set; }
    public DbSet<Basket_Item> Basket_Items { get; set; }

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options
        ) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // builder.ApplyConfiguration(new RoleConfiguration());
    }

}