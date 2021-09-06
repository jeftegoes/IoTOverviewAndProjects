using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Ordering.Models;

namespace Ordering.Persistence
{
    public class OrderContext : DbContext
    {
        public DbSet<Order> Order { get; set; }
        public DbSet<OrderDetail> OrderDetail { get; set; }

        public OrderContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            var converter = new EnumToStringConverter<Status>();
            builder
                .Entity<Order>()
                .Property(p => p.Status)
                .HasConversion(converter);
        }
    }
}