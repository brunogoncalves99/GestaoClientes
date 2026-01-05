using GestaoClientes.Application.Common.Results;
using GestaoClientes.Application.DTOs;
using MediatR;

namespace GestaoClientes.Application.Commands
{
    public record CriaClienteCommand(string NomeFantasia, string Cnpj) : IRequest<Result<ClienteDto>>;
}
