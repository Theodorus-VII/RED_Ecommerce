using Ecommerce.Data;
using Ecommerce.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Configuration;

public static class ApplicationDbContextSeed
{
    public static async Task<IServiceProvider> InitializeDb(this IServiceProvider services)
    {
        using (var context = new ApplicationDbContext(
            services.GetRequiredService<DbContextOptions<ApplicationDbContext>>()
        ))
        {
            var products = new List<Product>();
            // if there no products initialized in the db, create some seed data
            if (!context.Products.Any())
            {
                foreach (Category category in Enum.GetValues(typeof(Category)))
                {
                    // Create 50 items for each category
                    for (int i = 0; i < 50; i++)
                    {
                        var product = new Product
                        {
                            Name = $"Sample Product {i + 1} for {category}",
                            Brand = "Seed",
                            Count = 10, // Example count
                            Details = $"Seed product details for {category}",
                            Category = category,
                            Price = 10.5f, // Example price
                        };
                        products.Add(
                            product
                        );
                    }
                }
                context.Products.AddRange(products);
                context.SaveChanges();
            }


            products = context.Products.ToList();

            // Add images for all the products. This will be random-ish and the images will not match
            // the descriptions or the categories of the products. This is only for testing purposes

            foreach (var product in products)
            {
                string url = "";
                if (product.Category == Category.Electronics
                    || product.Category == Category.Automotive
                    || product.Category == Category.ElectronicsPhone
                    || product.Category == Category.ElectronicsPhone
                    || product.Category == Category.SoftwareEducation
                    || product.Category == Category.AutomotiveCarCare
                    || product.Category == Category.AutomotiveReplacementParts
                    || product.Category == Category.AutomotiveToolsAndEquipments)
                {
                    url = "elec_81Zt42ioCgL._AC_SX679_.jpg";
                }
                else
                {
                    url = "clothing_71-3HjGNDUL._AC_SY879._SX._UX._SY._UY_.jpg";
                }

                    product.Images = new List<Image>
                {
                    new Image
                    {
                        Url = url,
                        ProductId = product.Id,
                        Product = product
                    }
                };
            }

            var userManager = services.GetRequiredService<UserManager<User>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
            if (!context.Users.Any())
            {

                // define roles
                string[] roleNames = { "Customer", "Admin" };

                // ensure the roles exist
                foreach (var roleName in roleNames)
                {
                    if (!await roleManager.RoleExistsAsync(roleName))
                    {
                        await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
                    }
                }
                var customer = new User(
                        "customer@email.email",
                        "Customer",
                        "The Customer",
                        "Garfield's house",
                        "Kizaru");
                var admin = new User(
                        "admin@email.email",
                        "Admin",
                        "The Admin",
                        "Odie's house",
                        "Garp");
                await userManager.CreateAsync(
                    customer, "customerpassword"
                );
                await userManager.AddToRoleAsync(customer, Roles.Customer);
                await userManager.CreateAsync(
                    admin, "adminpassword"
                );
                await userManager.AddToRoleAsync(admin, Roles.Admin);

                // confirming the emails of the admin and customer default accounts
                await userManager.ConfirmEmailAsync(customer, await userManager.GenerateEmailConfirmationTokenAsync(customer));
                await userManager.ConfirmEmailAsync(admin, await userManager.GenerateEmailConfirmationTokenAsync(admin));
            }

            var users = await userManager.GetUsersInRoleAsync(Roles.Customer);

            var ratings = new List<Rating>();
            if (!context.Ratings.Any())
            {
                for (int i = 0; i < 100; i++) // Example: creating 100 ratings
                {
                    Random random = new Random();
                    ratings.Add(new Rating
                    {
                        RatingN = random.Next(6), // Random rating
                        Review = i % 2 == 0 ? "Great product!" : i % 3 == 0 ? "Amazing product!" : "Shite product. I'm suing this company", // Example review
                        UserId = users[i % users.Count].Id, // Assuming users is a list of User entities
                        ProductId = products[i % products.Count].Id // Assuming products is a list of Product entities
                    });
                }
                context.Ratings.AddRange(ratings);
                context.SaveChanges();
            }


            // if (!context.Carts.Any())
            // {
            //     var carts = new List<Cart>();
            //     for (int i = 0; i < 50; i++) // Example: creating 50 carts
            //     {
            //         var cart = new Cart
            //         {
            //             UserId = users[i % users.Count].Id.ToString(), // Assuming users is a list of User entities
            //             TotalPrice = 100.0f // Example total price
            //         };

            //         var cartItems = new List<CartItem>();
            //         for (int j = 0; j < 5; j++) // Example: each cart has 5 items
            //         {
            //             cartItems.Add(new CartItem
            //             {
            //                 ProductId = products[j % products.Count].Id, // Assuming products is a list of Product entities
            //                 Quantity = 1, // Example quantity
            //                 Price = products[j % products.Count].Price // Example price
            //             });
            //         }
            //         cart.Items = cartItems;
            //         carts.Add(cart);
            //     }
            //     context.Carts.AddRange(carts);
            //     context.SaveChanges();

            // }

            // if (!context.Orders.Any())
            // {
            //     var orders = new List<Order>();
            //     for (int i = 0; i < 50; i++) // Example: creating 50 orders
            //     {
            //         var order = new Order
            //         {
            //             UserId = users[i % users.Count].Id.ToString(), // Assuming users is a list of User entities
            //             OrderDate = DateTime.UtcNow.AddDays(-i), // Example order date
            //             TotalAmount = 100.0m // Example total amount
            //         };

            //         var orderItems = new List<OrderItem>();
            //         for (int j = 0; j < 5; j++) // Example: each order has 5 items
            //         {
            //             orderItems.Add(new OrderItem
            //             {
            //                 ProductId = products[j % products.Count].Id, // Assuming products is a list of Product entities
            //                 ProductName = products[j % products.Count].Name, // Example product name
            //                 Price = new decimal(products[j % products.Count].Price), // Example price
            //                 Quantity = 1 // Example quantity
            //             });
            //         }
            //         order.OrderItems = orderItems;
            //         orders.Add(order);
            //     }
            //     context.Orders.AddRange(orders);
            //     context.SaveChanges();
            // }

            // if (!context.PaymentInfos.Any())
            // {
            //     var paymentInfos = new List<PaymentInfo>();
            //     for (int i = 0; i < 50; i++) // Example: creating 50 payment info records
            //     {
            //         paymentInfos.Add(new PaymentInfo
            //         {
            //             UserId = users[i % users.Count].Id.ToString(), // Assuming users is a list of User entities
            //             CardNumber = "1234567890123456", // Example card number
            //             CardHolderName = "John Doe", // Example card holder name
            //             ExpiryDate = "12/25", // Example expiry date
            //             CVV = "123" // Example CVV
            //         });
            //     }
            //     context.PaymentInfos.AddRange(paymentInfos);
            //     context.SaveChanges();
            // }
        }
        return services;
    }
}