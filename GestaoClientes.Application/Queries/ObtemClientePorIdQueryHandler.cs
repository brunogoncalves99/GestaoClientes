using GestaoClientes.Application.Common.Results;
using GestaoClientes.Application.DTOs;
using GestaoClientes.Domain.Interfaces;
using MediatR;

namespace GestaoClientes.Application.Queries
{
    public class ObtemClientePorIdQueryHandler : IRequestHandler<ObtemClientePorIdQuery, Result<ClienteDto>>
    {
        private readonly IClienteRepository _clienteRepository;

        public ObtemClientePorIdQueryHandler(IClienteRepository clienteRepository)
        {
            _clienteRepository = clienteRepository;
        }

        public async Task<Result<ClienteDto>> Handle(ObtemClientePorIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var cliente = await _clienteRepository.ObterPorIdAsync(request.Id, cancellationToken);

                if (cliente is null)
                {
                    return Result<ClienteDto>.Falha("Cliente não encontrado.");
                }

                var clienteDto = new ClienteDto
                {
                    Id = cliente.Id,
                    NomeFantasia = cliente.NomeFantasia,
                    Cnpj = cliente.Cnpj.Valor,
                    CnpjFormatado = cliente.Cnpj.ObterFormatado(),
                    Ativo = cliente.Ativo,
                    DataCadastro = cliente.DataCadastro,
                    DataAtualizacao = cliente.DataAtualizacao
                };

                return Result<ClienteDto>.Ok(clienteDto, "Cliente obtido com sucesso.");
            }
            catch (Exception ex)
            {
                return Result<ClienteDto>.Falha($"Erro ao obter cliente: {ex.Message}");
            }
        }
    }
}
