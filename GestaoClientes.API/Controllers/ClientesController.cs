using GestaoClientes.Application.Commands;
using GestaoClientes.Application.DTOs;
using GestaoClientes.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GestaoClientes.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ClientesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ClientesController> _logger;

        public ClientesController(IMediator mediator, ILogger<ClientesController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Cria um novo cliente.
        /// </summary>
        /// <param name="command">Dados do cliente a ser criado</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Dados do cliente criado</returns>
        /// <response code="201">Cliente criado com sucesso</response>
        /// <response code="400">Dados inválidos ou CNPJ já cadastrado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPost]
        [ProducesResponseType(typeof(ClienteDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CriarCliente([FromBody] CriaClienteCommand command, CancellationToken cancellationToken) 
        {
            _logger.LogInformation("Iniciando criação de cliente com CNPJ: {Cnpj}", command.Cnpj);

            var resultado = await _mediator.Send(command, cancellationToken);

            if (!resultado.Sucesso)
            {
                _logger.LogWarning("Falha ao criar cliente: {Erros}", string.Join(", ", resultado.Erros));
                return BadRequest(new
                {
                    mensagem = resultado.Mensagem,
                    erros = resultado.Erros
                });
            }

            _logger.LogInformation("Cliente criado com sucesso. ID: {ClienteId}", resultado.Dados?.Id);

            return CreatedAtAction(
                nameof(ObterClientePorId),
                new { id = resultado.Dados!.Id },
                new
                {
                    mensagem = resultado.Mensagem,
                    dados = resultado.Dados
                });
        }

        /// <summary>
        /// Obtém um cliente por ID.
        /// </summary>
        /// <param name="id">ID do cliente</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Dados do cliente</returns>
        /// <response code="200">Cliente encontrado</response>
        /// <response code="404">Cliente não encontrado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ClienteDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObterClientePorId([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Consultando cliente com ID: {ClienteId}", id);

            var query = new ObtemClientePorIdQuery(id);
            var resultado = await _mediator.Send(query, cancellationToken);

            if (!resultado.Sucesso)
            {
                _logger.LogWarning("Cliente não encontrado. ID: {ClienteId}", id);
                return NotFound(new
                {
                    mensagem = resultado.Mensagem,
                    erros = resultado.Erros
                });
            }

            _logger.LogInformation("Cliente encontrado. ID: {ClienteId}", id);

            return Ok(new
            {
                mensagem = resultado.Mensagem,
                dados = resultado.Dados
            });
        }
    }
}
