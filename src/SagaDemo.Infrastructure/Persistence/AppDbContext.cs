using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;
using SagaDemo.Infrastructure.Saga;

namespace SagaDemo.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<OrderSagaState> OrderSagaStates => Set<OrderSagaState>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<OrderSagaState>(entity =>
        {
            entity.HasKey(e => e.CorrelationId);
            entity.Property(e => e.CurrentState).HasMaxLength(64).IsRequired();
            entity.Property(e => e.CustomerEmail).HasMaxLength(256);
            entity.Property(e => e.ProductName).HasMaxLength(256);
            entity.Property(e => e.TotalAmount).HasPrecision(18, 2);
            entity.Property(e => e.FailureReason).HasMaxLength(1000);
            entity.Property(e => e.TrackingNumber).HasMaxLength(100);
            entity.Property(e => e.RowVersion).IsRowVersion();
        });

        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();
    }
}
