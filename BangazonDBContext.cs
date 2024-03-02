using System;
using Microsoft.EntityFrameworkCore;
using bangAzon.Models;


public class BangazonDBContext : DbContext
{
    public DbSet<Category> Categories { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<PaymentType> PaymentTypes { get; set; }
    public DbSet<ProductOrder> ProductOrders { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<User> Users { get; set; }

    public BangazonDBContext(DbContextOptions<BangazonDBContext> context) : base(context)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>().HasData(new Category[]
        {
           new Category {id = 1, name = "Elecronics"},
           new Category {id = 2, name = "Music Video"},
           new Category {id = 3, name = "Groceries"}
        });

        modelBuilder.Entity<Order>().HasData(new Order[]
        {
            new Order {id = 1, customerId = 1, paymentTypeId = 1, orderStatus = true, orderDate = new DateTime (2022,05,07)},
            new Order {id = 2, customerId = 2, paymentTypeId = 2, orderStatus = true, orderDate = new DateTime (2023,04,08)},
            new Order {id = 3, customerId = 3, paymentTypeId = 3, orderStatus = true, orderDate = new DateTime (2022,06,09) },
        });

        modelBuilder.Entity<PaymentType>().HasData(new PaymentType[]
        {
            new PaymentType {id = 1, name = "Cash"},
            new PaymentType {id = 2, name = "Debit Card"},
            new PaymentType {id = 3, name = "Visa"},
            new PaymentType {id = 4, name = "American Express"},
            new PaymentType {id = 5, name = "Master Card"},
            new PaymentType {id = 6, name = "PayPal"},
            new PaymentType {id = 7, name = "Vinmo"},
        });

        modelBuilder.Entity<Product>().HasData(new Product[]
        {
            new Product {id = 1, title = "Dell 5400", description = "laptop computer", quantity = 25, unitPrice = 1154.00M, categoryId = 1, sellerId = 1, createDate = new DateTime(2023-01-07)},
            new Product {id = 2, title = "Art of Noise by Enigma", description = "music", quantity = 102, unitPrice = 9.99M, categoryId = 2, sellerId = 2, createDate = new DateTime(2022-01-10)},
            new Product {id = 3, title = "Gain Laundry Detergent", description = "laundry detergent", quantity = 59, unitPrice = 21.35M, categoryId = 3, sellerId = 2, createDate =  new DateTime(2022-01-08)},
        });

        modelBuilder.Entity<ProductOrder>().HasData(new ProductOrder[]
        {
            new ProductOrder {id = 1, productId = 1, orderId = 1},
            new ProductOrder {id = 2, productId = 2, orderId = 2},
            new ProductOrder {id = 3, productId = 3, orderId = 3},
            new ProductOrder {id = 4, productId = 1, orderId = 3}
        });

        modelBuilder.Entity<User>().HasData(new User[]
        {
            new User {userId = 1, name = "John Doe", email = "johndoe@bangazon.com", isSeller = true, storeId = 1 },
            new User {userId = 2, name = "Jane Doe", email = "janedoe@bangazon.com", isSeller = true, storeId = 1 },
            new User {userId = 3, name = "Luca Bonini", email = "lbonini@bangazon.com", isSeller = false, storeId = 1 },
            

        });
        modelBuilder.Entity<PaymentType>()
               .HasOne(pt => pt.users)
               .WithMany(u => u.paymentType)
               .HasForeignKey(pt => pt.userId); 

        base.OnModelCreating(modelBuilder);
    }
}

