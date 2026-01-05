using GestaoClientes.Application.Common.Results;
using GestaoClientes.Application.DTOs;
using GestaoClientes.Domain.Entities;
using GestaoClientes.Domain.Exceptions;
using GestaoClientes.Domain.Interfaces;
using GestaoClientes.Domain.ValueObjects;
using MediatR;


namespace GestaoClientes.Application.Commands
{
    public class CriaClienteCommandHandler : IRequestHandler<CriaClienteCommand, Result<ClienteDto>>
    {
        private readonly IClienteRepository _clienteRepository;

        public CriaClienteCommandHandler(IClienteRepository clienteRepository)
        {
            _clienteRepository = clienteRepository;
        }

        public async Task<Result<ClienteDto>> Handle(CriaClienteCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var cnpj = Cnpj.Criar(request.Cnpj);

                var clienteExistente = await _clienteRepository.ObterPorCnpjAsync(cnpj, cancellationToken);
                if (clienteExistente != null)
                {
                    return Result<ClienteDto>.Falha("Já existe um cliente cadastrado com este CNPJ.");
                }

                var cliente = Cliente.Criar(request.NomeFantasia, cnpj);

                await _clienteRepository.AdicionarAsync(cliente, cancellationToken);

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

                return Result<ClienteDto>.Ok(clienteDto, "Cliente criado com sucesso.");
            }
            catch (DomainException ex)
            {
                return Result<ClienteDto>.Falha(ex.Message);
            }
            catch (Exception ex)
            {
                return Result<ClienteDto>.Falha($"Erro ao criar cliente: {ex.Message}");
            }
        }
    }
}
