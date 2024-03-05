using bangAzon.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// allows passing datetimes without time zone data 
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// allows our api endpoints to access the database through Entity Framework Core
builder.Services.AddNpgsql<BangazonDBContext>(builder.Configuration["BangAzonDbConnectionString"]);

// Set the JSON serializer options
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
//Customer should be able to view the products
app.MapGet("/api/products", (BangazonDBContext db) =>
{
    return db.Products.ToList();
});
//Get single product
app.MapGet("/api/products/{id}", (BangazonDBContext db, int id) =>
{
    return db.Products.FirstOrDefault(p => p.id == id);
});

app.MapPost("/api/orders", (BangazonDBContext db, Order order) =>
{
    db.Orders.Add(order);
    db.SaveChanges();
    return Results.Created($"/api/order/{order.id}", order);
});

app.MapDelete("/api/singleorder/{id}", (BangazonDBContext db, int id) =>
{
    Order order = db.Orders.SingleOrDefault(order => order.id == id);
    if (order == null)
    {
        return Results.NotFound();
    }
    db.Orders.Remove(order);
    return Results.NoContent();
});

app.MapGet("/api/orders/{id}", (BangazonDBContext db, int id) =>
{
    var order = db.Orders.SingleOrDefault(o => o.id == id);

    if (order == null)
    {
        return Results.NotFound("No Orders");
    }

    return Results.Ok(order);
});
//Allow user to view Product Details
app.MapGet("/api/productdetails/{id}", (BangazonDBContext db, int id) =>
{
    var product = db.Products
        .Include(p => p.categoryId) 
        .SingleOrDefault(p => p.id == id);

    if (product == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(product);
});
//Customer can view their completed orders
app.MapGet("/api/customers/{customerId}/orders/completed", (BangazonDBContext db, int customerId) =>
{
    var completedOrders = db.Orders
        .Where(o => o.customerId == customerId && o.orderStatus) 
        //.Include(o => o.Products)
            //.ThenInclude(op => op.productOrders) 
        .OrderByDescending(o => o.orderDate)
        .ToList();

    return Results.Ok(completedOrders);
});
//Seller dashboard
//app.MapGet("/api/seller/dashboard", (BangazonDBContext db, HttpContext httpContext) =>
//{
//    // Get the seller ID from the authenticated user
//    var loggedInSellerId = httpContext.User.Claims.FirstOrDefault(c => c.Type == "SellerId")?.Value;

//    if (string.IsNullOrEmpty(loggedInSellerId))
//    {
//        // Return an unauthorized response or handle the case where the seller ID is not found
//        return Results.Unauthorized();
//    }

//    int sellerId = int.Parse(loggedInSellerId);

//    // Calculate Total Sales
//    var totalSales = db.ProductOrders
//        .Where(o => o.products.sellerId == sellerId && o.orders.orderStatus)
//        .Sum(o => o.products.TotalPrice);

//    // Calculate Total Sales for this month
//    var totalSalesThisMonth = db.ProductOrders
//        .Where(o => o.products.sellerId == sellerId && o.orders.orderStatus && o.orders.orderDate.Month == DateTime.Now.Month && o.orders.orderDate.Year == DateTime.Now.Year)
//        .Sum(o => o.products.TotalPrice);

//    // Calculate Average per item
//    var totalItemsSold = db.ProductOrders
//        .Where(op => op.products.sellerId == sellerId && op.orders.orderStatus)
//        .Sum(op => op.products.quantity);

//    var averagePerItem = totalItemsSold != 0 ? totalSales / totalItemsSold : 0;

//    // Total Inventory by Category
//    var totalInventoryByCategory = db.Products
//        .Where(p => p.sellerId == sellerId)
//        .GroupBy(p => p.categoryId)
//        .Select(g => new TotalInventoryByCategoryDTO
//        {
//            CategoryId = g.Key,
//            TotalInventory = g.Sum(p => p.quantity)
//        })
//        .ToList();

//    // Orders that require shipping
//    var ordersRequiringShipping = db.Orders
//        .Where(o => o.sellerId == sellerId && o.orderStatus && o.requiresShipping)
//        .Select(o => new
//        {
//            o.id,
//            o.customerId,
//            o.paymentTypeId,
//            o.orderStatus,
//            o.orderDate,
//            o.sellerId
//        })
//        .ToList();

//    var dashboardData = new
//    {
//        TotalSales = new TotalSalesDTO { TotalSales = totalSales },
//        TotalSalesThisMonth = new TotalSalesThisMonthDTO { TotalSalesThisMonth = totalSalesThisMonth },
//        AveragePerItem = new AveragePerItemDTO { AveragePerItem = averagePerItem },
//        TotalInventoryByCategory = totalInventoryByCategory,
//        OrdersRequiringShipping = ordersRequiringShipping
//    };

//    return Results.Ok(dashboardData);
//});
//Search for products based on the provided search terms
app.MapGet("api/products/search", (BangazonDBContext db, string searchTerm) =>
{
    var matchProducts = db.Products
    .Where(p => p.title.Contains(searchTerm) || p.description.Contains(searchTerm)).ToList();

    if (matchProducts.Count == 0)
    {
        return Results.NotFound("No products matched the search criteria");
    }
    return Results.Ok(matchProducts);

});
// Customers can view all products sold by a specific seller
app.MapGet("/api/sellers/{sellerId}/products", (BangazonDBContext db, int sellerId) =>
{
    // Retrieve all products sold by the specified seller
    var sellerProducts = db.Products
        .Where(p => p.sellerId == sellerId)
        .ToList();

    if (sellerProducts.Count == 0)
    {
        return Results.NotFound("No products found for the specified seller.");
    }

    return Results.Ok(sellerProducts);
});

// Customers can view lastest products on home page
app.MapGet("/api/products/latest", (BangazonDBContext db) =>
{
    // Retrieve the latest products based on their creation date
    var latestProducts = db.Products
        .OrderByDescending(p => p.createDate)
        .Take(10) // Assuming you want to retrieve the latest 10 products
        .ToList();

    if (latestProducts.Count == 0)
    {
        return Results.NotFound("No latest products found.");
    }

    return Results.Ok(latestProducts);
});

// Customers can view there order history
app.MapGet("/api/customers/{customerId}/orders", (BangazonDBContext db, int customerId) =>
{
    // Retrieve the order history of the specified customer
    var orderHistory = db.Orders
        .Where(o => o.customerId == customerId)
        //.Include(o => o.Products)
         //   .ThenInclude(op => op.productOrders)
        .OrderByDescending(o => o.orderDate)
        .ToList();

    if (orderHistory.Count == 0)
    {
        return Results.NotFound("No orders found for the specified customer.");
    }

    return Results.Ok(orderHistory);
});

// Customer can add products
app.MapPost("/api/products", async (BangazonDBContext db, Product product) =>
{
    // Check if the product data is valid
    if (product == null || string.IsNullOrWhiteSpace(product.title) || product.unitPrice <= 0)
    {
        return Results.BadRequest("Invalid product data. Please provide a name and a valid price.");
    }

    // Add the product to the database
    db.Products.Add(product);
    await db.SaveChangesAsync();

    return Results.Created($"/api/products/{product.id}", product);
});
//Seller can view his past sells
//app.MapGet("/api/sellers/{sellerId}/sales", (BangazonDBContext db, int sellerId) =>
//{
//    // Retrieve past sales made by the specified seller
//    var sellerSales = db.ProductOrders
//        //.Where(po => po.products.sellerId == sellerId && po.orders.orderStatus)
//        .Select(po => new
//        {
//            OrderId = po.orders.id,
//            ProductId = po.productId,
//            ProductName = po.products.title,
//            Quantity = po.products.quantity,
//            TotalPrice = po.products.TotalPrice,
//            OrderDate = po.orders.orderDate
//        })
//        .OrderByDescending(po => po.OrderDate)
//        .ToList();

//    if (sellerSales.Count == 0)
//    {
//        return Results.NotFound("No sales found for the specified seller.");
//    }

//    return Results.Ok(sellerSales);
//});
// Users can view all product categories
app.MapGet("/api/categories", (BangazonDBContext db) =>
{
    // Retrieve all product categories
    var categories = db.Categories.ToList();

    if (categories.Count == 0)
    {
        return Results.NotFound("No categories found.");
    }

    return Results.Ok(categories);
});
//Customers can chose their payment method
app.MapPost("/api/orders/checkout", async (BangazonDBContext db, HttpContext httpContext, int paymentTypeId) =>
{
    // Assuming you have a logged-in customer, retrieve their ID from the authentication token
    // For demonstration purposes, let's assume the customer ID is obtained from the request header
    if (!httpContext.Request.Headers.ContainsKey("CustomerId"))
    {
        return Results.BadRequest("Customer ID is missing from the request header.");
    }

    int customerId;
    if (!int.TryParse(httpContext.Request.Headers["CustomerId"], out customerId))
    {
        return Results.BadRequest("Invalid customer ID format in the request header.");
    }

    // Retrieve the customer's payment types
    var customerPaymentTypes = await db.PaymentTypes
        .Where(pt => pt.users.userId == customerId)
        .ToListAsync();

    //// Check if the specified payment type belongs to the customer
    //var selectedPaymentType = customerPaymentTypes.FirstOrDefault(pt => pt.id == paymentTypeId);
    //if (selectedPaymentType == null)
    //{
    //    return Results.BadRequest("Invalid payment type selected. Please choose a payment type associated with the customer.");
    //}

    //// Get the customer's shopping cart
    //var shoppingCart = await db.ProductOrders
    //    .Include(sc => sc.products)
    //    .FirstOrDefaultAsync(sc => sc.orders.customerId == customerId);

    //if (shoppingCart == null )
    //{
    //    return Results.BadRequest("Shopping cart is empty.");
    //}

    // Create an order from the shopping cart
    var order = new Order
    {
        customerId = customerId,
        paymentTypeId = paymentTypeId,
        orderDate = DateTime.UtcNow,
        
    };

    // Add the order to the database
    db.Orders.Add(order);
    await db.SaveChangesAsync();

    //// Clear the customer's shopping cart
    //db.ProductOrders.Remove(shoppingCart);
    //await db.SaveChangesAsync();

    return Results.Created($"/api/orders/{order.id}", order);
});
// Customer can search for a Seller name
app.MapGet("/api/sellers/search", (BangazonDBContext db, string searchText) =>
{
    // Search for sellers based on the provided search text
    var matchedSellers = db.Users
        .Where(s => s.name.Contains(searchText))
        .ToList();

    if (matchedSellers.Count == 0)
    {
        return Results.NotFound("No sellers found matching the search text.");
    }

    return Results.Ok(matchedSellers);
});

// Customer can view their profile
app.MapGet("/api/customers/{customerId}/profile", (BangazonDBContext db, int customerId) =>
{
    // Retrieve the customer's profile based on their ID
    var customer = db.Users
        .Include(c => c.paymentType)
        .FirstOrDefault(c => c.userId == customerId);

    if (customer == null)
    {
        return Results.NotFound("Customer not found.");
    }

    return Results.Ok(customer);
});

//User can register for BangAzon
app.MapPost("/api/register", async (BangazonDBContext db, User user) =>
{

    // Check if the username is already taken
    if (await db.Users.AnyAsync(u => u.name == user.name))
    {
        return Results.Conflict("Username is already taken.");
    }

    // Create a new user entity
    var newUser = new User
    {
        name = user.name,
        email = user.email,

    };

    // Add the new user to the database
    db.Users.Add(newUser);
    await db.SaveChangesAsync();

    // Return a success response
    return Results.Created($"/api/users/{newUser.userId}", newUser);
});

app.Run();


