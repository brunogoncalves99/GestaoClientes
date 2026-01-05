using GestaoClientes.Domain.Entities;
using GestaoClientes.Domain.ValueObjects;

namespace GestaoClientes.Domain.Interfaces
{
    public interface IClienteRepository
    {
        Task AdicionarAsync(Cliente cliente, CancellationToken cancellationToken = default);
        Task<Cliente?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Cliente?> ObterPorCnpjAsync(Cnpj cnpj, CancellationToken cancellationToken = default);
        Task<bool> ExistePorCnpjAsync(Cnpj cnpj, CancellationToken cancellationToken = default);
        Task<IEnumerable<Cliente>> ObterTodosAsync(CancellationToken cancellationToken = default);
        Task AtualizarAsync(Cliente cliente, CancellationToken cancellationToken = default);
        Task RemoverAsync(Cliente cliente, CancellationToken cancellationToken = default);
    }
}