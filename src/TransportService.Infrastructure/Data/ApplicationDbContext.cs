using Microsoft.EntityFrameworkCore;
using TransportService.Core.Entities;
using TransportService.Infrastructure.Data.Configurations;

namespace TransportService.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Transport> Transports { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new TransportConfiguration());
    }
}