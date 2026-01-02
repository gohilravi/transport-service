using Microsoft.EntityFrameworkCore;
using TransportService.Core.Entities;
using TransportService.Core.Interfaces;
using TransportService.Infrastructure.Data;

namespace TransportService.Infrastructure.Repositories;

public class TransportRepository : ITransportRepository
{
    private readonly ApplicationDbContext _context;

    public TransportRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Transport?> GetByIdAsync(int id)
    {
        return await _context.Transports.FindAsync(id);
    }

    public async Task<Transport> CreateAsync(Transport transport)
    {
        transport.CreatedAt = DateTime.UtcNow;
        transport.LastModifiedAt = DateTime.UtcNow;
        
        var entry = await _context.Transports.AddAsync(transport);
        return entry.Entity;
    }

    public Task<Transport> UpdateAsync(Transport transport)
    {
        transport.LastModifiedAt = DateTime.UtcNow;
        
        _context.Entry(transport).State = EntityState.Modified;
        return Task.FromResult(transport);
    }
}