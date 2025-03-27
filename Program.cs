// using Microsoft.AspNetCore.Builder;
// using Microsoft.AspNetCore.Hosting;
// using Microsoft.AspNetCore.Http;
// using Microsoft.Extensions.DependencyInjection;
// using Microsoft.Extensions.Hosting;
// using System.Collections.Generic;
// using System.IO;
// using System.Text.Json;
// using System.Linq;
// using System.Text.Json.Serialization;

// var builder = WebApplication.CreateBuilder(args);
// builder.Services.AddRazorPages();
// var app = builder.Build();

// app.UseStaticFiles();
// app.UseRouting();

// app.UseEndpoints(endpoints =>
// {
//     endpoints.MapGet("/", async context =>
//     {
//         await context.Response.WriteAsync("<h1>Welcome</h1><a href='/store'>Store</a><br/><a href='/orders'>My Orders</a>");
//     });

//     endpoints.MapGet("/store", async context =>
//     {
//         if (!File.Exists("products.json"))
//         {
//             await context.Response.WriteAsync("<h1>Error: products.json not found</h1>");
//             return;
//         }

//         var productsJson = await File.ReadAllTextAsync("products.json");
//         var products = JsonSerializer.Deserialize<List<Product>>(productsJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<Product>();

//         var html = "<h1>Store</h1><ul>";
//         foreach (var product in products)
//         {
//             html += $"<li>{product.ProductName} - ${product.Cost}</li>";
//         }
//         html += "</ul><a href='/'>Back</a>";

//         await context.Response.WriteAsync(html);
//     });

//     endpoints.MapGet("/orders", async context =>
//     {
//         if (!File.Exists("orders.json"))
//         {
//             await context.Response.WriteAsync("<h1>Error: orders.json not found</h1>");
//             return;
//         }

//         var ordersJson = await File.ReadAllTextAsync("orders.json");
//         var orders = JsonSerializer.Deserialize<List<Order>>(ordersJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<Order>();

//         var html = "<h1>My Orders</h1><ul>";
//         foreach (var order in orders)
//         {
//             var totalNetPrice = order.Items?.Sum(i => i.NetCost) ?? 0;
//             html += $"<li>Order ID: {order.Id} - Net Price: ${totalNetPrice}</li>";
//         }
//         html += "</ul><a href='/'>Back</a>";

//         await context.Response.WriteAsync(html);
//     });
// });

// app.Run();

// public class Product
// {
//     [JsonPropertyName("product_id")]
//     public int ProductId { get; set; }

//     [JsonPropertyName("product_name")]
//     public string ProductName { get; set; }

//     [JsonPropertyName("cost")]
//     public decimal Cost { get; set; }
// }

// public class Order
// {
//     [JsonPropertyName("id")]
//     public int Id { get; set; }

//     [JsonPropertyName("items")]
//     public List<OrderItem> Items { get; set; } = new List<OrderItem>();
// }

// public class OrderItem
// {
//     [JsonPropertyName("productId")]
//     public int ProductId { get; set; }

//     [JsonPropertyName("netCost")]
//     public decimal NetCost { get; set; }
// }

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text;
using System.Linq;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapGet("/", async context =>
    {
        var homeHtml = new StringBuilder();
        homeHtml.Append("<h1>Welcome</h1>");
        homeHtml.Append("<a href='/store'>Store</a><br/>");
        homeHtml.Append("<a href='/orders'>My Orders</a>");
        await context.Response.WriteAsync(homeHtml.ToString());
    });

    endpoints.MapGet("/store", async context =>
    {
        if (!File.Exists("products.json"))
        {
            await context.Response.WriteAsync("<h1>Error: products.json not found</h1>");
            return;
        }

        string jsonString = await File.ReadAllTextAsync("products.json");
        List<Product>? productList = JsonSerializer.Deserialize<List<Product>>(jsonString);

        if (productList == null)
        {
            await context.Response.WriteAsync("<h1>Error: Failed to load products</h1>");
            return;
        }

        var storeHtml = new StringBuilder();
        storeHtml.Append("<h1>Store</h1><ul>");
        
        foreach (var product in productList)
        {
            storeHtml.Append("<li>" + product.ProductName + " - $" + product.Cost + "</li>");
        }

        storeHtml.Append("</ul><a href='/'>Back</a>");
        await context.Response.WriteAsync(storeHtml.ToString());
    });

    endpoints.MapGet("/orders", async context =>
    {
        if (!File.Exists("orders.json"))
        {
            await context.Response.WriteAsync("<h1>Error: orders.json not found</h1>");
            return;
        }

        string jsonString = await File.ReadAllTextAsync("orders.json");
        List<Order>? orderList = JsonSerializer.Deserialize<List<Order>>(jsonString);

        if (orderList == null)
        {
            await context.Response.WriteAsync("<h1>Error: Failed to load orders</h1>");
            return;
        }

        var ordersHtml = new StringBuilder();
        ordersHtml.Append("<h1>My Orders</h1><ul>");

        foreach (var order in orderList)
        {
            decimal totalNetPrice = 0;
            foreach (var item in order.Items)
            {
                totalNetPrice += item.NetCost;
            }
            ordersHtml.Append("<li>Order ID: " + order.Id + " - Net Price: $" + totalNetPrice + "</li>");
        }

        ordersHtml.Append("</ul><a href='/'>Back</a>");
        await context.Response.WriteAsync(ordersHtml.ToString());
    });
});

app.Run();

public class Product
{
    [JsonPropertyName("product_id")]
    public int ProductId { get; set; }

    [JsonPropertyName("product_name")]
    public string ProductName { get; set; }

    [JsonPropertyName("cost")]
    public decimal Cost { get; set; }
}

public class Order
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("items")]
    public List<OrderItem> Items { get; set; } = new List<OrderItem>();
}

public class OrderItem
{
    [JsonPropertyName("productId")]
    public int ProductId { get; set; }

    [JsonPropertyName("netCost")]
    public decimal NetCost { get; set; }
}
