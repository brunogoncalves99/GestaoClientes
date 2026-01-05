using GestaoClientes.Domain.Entities;
using GestaoClientes.Domain.Interfaces;
using GestaoClientes.Domain.ValueObjects;
using GestaoClientes.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GestaoClientes.Infrastructure.Repositories;

public class ClienteRepository : IClienteRepository
{
    private readonly ClientesDbContext _context;

    public ClienteRepository(ClientesDbContext context)
    {
        _context = context;
    }

    public async Task AdicionarAsync(Cliente cliente, CancellationToken cancellationToken = default)
    {
        await _context.Clientes.AddAsync(cliente, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<Cliente?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Clientes
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<Cliente?> ObterPorCnpjAsync(Cnpj cnpj, CancellationToken cancellationToken = default)
    {
        return await _context.Clientes
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Cnpj == cnpj, cancellationToken);
    }

    public async Task<bool> ExistePorCnpjAsync(Cnpj cnpj, CancellationToken cancellationToken = default)
    {
        return await _context.Clientes
            .AsNoTracking()
            .AnyAsync(c => c.Cnpj == cnpj, cancellationToken);
    }

    public async Task<IEnumerable<Cliente>> ObterTodosAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Clientes
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task AtualizarAsync(Cliente cliente, CancellationToken cancellationToken = default)
    {
        _context.Clientes.Update(cliente);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoverAsync(Cliente cliente, CancellationToken cancellationToken = default)
    {
        _context.Clientes.Remove(cliente);
        await _context.SaveChangesAsync(cancellationToken);
    }
}