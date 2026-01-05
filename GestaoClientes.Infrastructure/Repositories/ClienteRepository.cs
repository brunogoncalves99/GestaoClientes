using GestaoClientes.Domain.Entities;
using GestaoClientes.Domain.Interfaces;
using GestaoClientes.Domain.ValueObjects;

namespace GestaoClientes.Infrastructure.Repositories
{
    public class ClienteRepository : IClienteRepository
    {
        private static readonly List<Cliente> _clientes = new();
        private static readonly object _lock = new();

        public Task AdicionarAsync(Cliente cliente, CancellationToken cancellationToken = default)
        {
            lock (_lock)
            {
                _clientes.Add(cliente);
            }
            return Task.CompletedTask;
        }
        public Task<Cliente?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            lock (_lock)
            {
                var cliente = _clientes.FirstOrDefault(c => c.Id == id);
                return Task.FromResult(cliente);
            }
        }

        public Task<Cliente?> ObterPorCnpjAsync(Cnpj cnpj, CancellationToken cancellationToken = default)
        {
            lock (_lock)
            {
                var cliente = _clientes.FirstOrDefault(c => c.Cnpj == cnpj);
                return Task.FromResult(cliente);
            }
        }

        public Task<bool> ExistePorCnpjAsync(Cnpj cnpj, CancellationToken cancellationToken = default)
        {
            lock (_lock)
            {
                var existe = _clientes.Any(c => c.Cnpj == cnpj);
                return Task.FromResult(existe);
            }
        }

        public Task<IEnumerable<Cliente>> ObterTodosAsync(CancellationToken cancellationToken = default)
        {
            lock (_lock)
            {
                var clientes = _clientes.ToList();
                return Task.FromResult<IEnumerable<Cliente>>(clientes);
            }
        }

        public Task AtualizarAsync(Cliente cliente, CancellationToken cancellationToken = default)
        {
            lock (_lock)
            {
                var clienteExistente = _clientes.FirstOrDefault(c => c.Id == cliente.Id);
                if (clienteExistente is not null)
                {
                    _clientes.Remove(clienteExistente);
                    _clientes.Add(cliente);
                }
            }
            return Task.CompletedTask;
        }

        public Task RemoverAsync(Cliente cliente, CancellationToken cancellationToken = default)
        {
            lock (_lock)
            {
                _clientes.Remove(cliente);
            }
            return Task.CompletedTask;
        }

        public static void LimparDados()
        {
            lock (_lock)
            {
                _clientes.Clear();
            }
        }
    }
}
