using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TransportService.Core.Entities;

namespace TransportService.Infrastructure.Data.Configurations;

public class TransportConfiguration : IEntityTypeConfiguration<Transport>
{
    public void Configure(EntityTypeBuilder<Transport> builder)
    {
        builder.ToTable("Transport");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .UseIdentityColumn();

        builder.Property(t => t.CarrierId)
            .IsRequired();

        builder.Property(t => t.PurchaseId)
            .IsRequired();

        builder.Property(t => t.PickupLocation)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(t => t.DeliveryLocation)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(t => t.ScheduleDate)
            .HasColumnType("timestamp with time zone");

        builder.Property(t => t.VehicleDetails)
            .HasColumnType("text");

        builder.Property(t => t.Status)
            .HasMaxLength(30)
            .HasDefaultValue("Scheduled");

        builder.Property(t => t.CreatedAt)
            .HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("(NOW() AT TIME ZONE 'UTC')");

        builder.Property(t => t.LastModifiedAt)
            .HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("(NOW() AT TIME ZONE 'UTC')");

        // Add indexes for better query performance
        builder.HasIndex(t => t.CarrierId);
        builder.HasIndex(t => t.PurchaseId);
        builder.HasIndex(t => t.Status);
        builder.HasIndex(t => t.ScheduleDate);

        // Configure cascade delete behavior for future related entities
        // Example: if Transport has related entities like TransportItems
        // builder.HasMany(t => t.TransportItems)
        //     .WithOne(ti => ti.Transport)
        //     .HasForeignKey(ti => ti.TransportId)
        //     .OnDelete(DeleteBehavior.Cascade);

        // Configure table with check constraint
        builder.ToTable(t => t.HasCheckConstraint(
            "CK_Transports_Status",
            "\"Status\" IN ('Assigned', 'InTransit', 'Completed', 'Canceled', 'Scheduled')"
        ));
    }
}