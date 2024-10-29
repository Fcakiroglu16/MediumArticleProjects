using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();


app.MapHealthChecks("/health/liveness").AllowAnonymous(); // Liveness Probe
app.MapHealthChecks("/health/readiness").AllowAnonymous(); // Readiness Probe


#region Product Endpoints

var products = new List<Product>()
{
    new Product { Id = 1, Name = "Keyboard", Price = 20 },
    new Product { Id = 2, Name = "Mouse", Price = 10 },
    new Product { Id = 3, Name = "Monitor", Price = 100 },
    new Product { Id = 4, Name = "Laptop", Price = 500 },
    new Product { Id = 5, Name = "Tablet", Price = 300 }
};

app.MapGet("/products", () => Results.Ok(products));

app.MapGet("/products/{id}", (int id) =>
{
    var product = products.FirstOrDefault(p => p.Id == id);
    return product is not null ? Results.Ok(product) : Results.NotFound();
});

app.MapPost("/products", (Product product) =>
{
    product.Id = products.Count > 0 ? products.Max(p => p.Id) + 1 : 1;
    products.Add(product);
    return Results.Created($"/products/{product.Id}", product);
});

app.MapPut("/products/{id}", (int id, Product updatedProduct) =>
{
    var product = products.FirstOrDefault(p => p.Id == id);
    if (product is null)
    {
        return Results.NotFound();
    }

    product.Name = updatedProduct.Name;
    product.Price = updatedProduct.Price;

    return Results.NoContent();
});

app.MapDelete("/products/{id}", (int id) =>
{
    var product = products.FirstOrDefault(p => p.Id == id);
    if (product is null)
    {
        return Results.NotFound();
    }

    products.Remove(product);
    return Results.NoContent();
});

#endregion


app.Run();


internal record Product
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public decimal Price { get; set; }
}