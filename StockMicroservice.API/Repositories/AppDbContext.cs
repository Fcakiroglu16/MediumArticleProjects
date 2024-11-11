using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace StockMicroservice.API.Repositories;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Stock> Stocks { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Stock>().HasKey(s => s.ProductId);


        //dump data
        modelBuilder.Entity<Stock>().HasData(new Stock
        {
            ProductId = 1,
            Quantity = 100
        });


        base.OnModelCreating(modelBuilder);

        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();
    }
}