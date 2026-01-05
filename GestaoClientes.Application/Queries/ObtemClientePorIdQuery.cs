using GestaoClientes.Application.Common.Results;
using GestaoClientes.Application.DTOs;
using MediatR;

namespace GestaoClientes.Application.Queries;

public record ObtemClientePorIdQuery(Guid Id) : IRequest<Result<ClienteDto>>;

