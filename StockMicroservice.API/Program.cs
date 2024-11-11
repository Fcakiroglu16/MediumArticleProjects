using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Events;
using StockMicroservice.API.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();
    x.AddEntityFrameworkOutbox<AppDbContext>(o =>
    {
        // configure which database lock provider to use (Postgres, SqlServer, or MySql)
        o.UseSqlServer();

        // enable the bus outbox
        o.UseBusOutbox();
    });
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });


        cfg.ConfigureEndpoints(context);
    });
});
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapPost("/stock/decrease",
    async (IPublishEndpoint publishEndpoint, AppDbContext context, DecreaseStockRequest request) =>
    {
        var stock = context.Stocks.FirstOrDefault(x => x.ProductId == request.ProductId);
        if (stock == null) return Results.NotFound();


        if (stock.Quantity < request.Quantity) return Results.BadRequest("Not enough stock");


        stock.Quantity -= request.Quantity;


        await publishEndpoint.Publish(new StockReservedEvent(request.OrderId));
        context.SaveChanges();


        return Results.Ok();
    });


app.Run();

public class DecreaseStockRequest
{
    public Guid OrderId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}