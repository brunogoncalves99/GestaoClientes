using FluentAssertions;
using GestaoClientes.Application.Commands;
using GestaoClientes.Domain.Entities;
using GestaoClientes.Domain.Interfaces;
using GestaoClientes.Domain.ValueObjects;
using Moq;

namespace GestaoClientes.Tests.Application.Commands
{
    public class CriaClienteCommandHandlerTests
    {
        private readonly Mock<IClienteRepository> _repositoryMock;
        private readonly CriaClienteCommandHandler _handler;

        public CriaClienteCommandHandlerTests()
        {
            _repositoryMock = new Mock<IClienteRepository>();
            _handler = new CriaClienteCommandHandler(_repositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ComDadosValidos_DeveCriarClienteComSucesso()
        {
            // Arrange
            var command = new CriaClienteCommand("Empresa Teste LTDA", "11.222.333/0001-81");

            _repositoryMock
                .Setup(x => x.ObterPorCnpjAsync(It.IsAny<Cnpj>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Cliente?)null);

            // Act
            var resultado = await _handler.Handle(command, CancellationToken.None);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Sucesso.Should().BeTrue();
            resultado.Mensagem.Should().Be("Cliente criado com sucesso.");
            resultado.Dados.Should().NotBeNull();
            resultado.Dados!.NomeFantasia.Should().Be("Empresa Teste LTDA");
            resultado.Dados.Cnpj.Should().Be("11222333000181");
            resultado.Dados.CnpjFormatado.Should().Be("11.222.333/0001-81");
            resultado.Dados.Ativo.Should().BeTrue();

            _repositoryMock.Verify(x => x.AdicionarAsync(It.IsAny<Cliente>(), It.IsAny<CancellationToken>()),Times.Once);
        }

        [Fact]
        public async Task Handle_ComCnpjJaExistente_DeveRetornarErro()
        {
            // Arrange
            var command = new CriaClienteCommand("Empresa Teste LTDA", "11.222.333/0001-81");
            var cnpj = Cnpj.Criar("11.222.333/0001-81");
            var clienteExistente = Cliente.Criar("Empresa Existente", cnpj);

            _repositoryMock
                .Setup(x => x.ObterPorCnpjAsync(It.IsAny<Cnpj>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(clienteExistente);

            // Act
            var resultado = await _handler.Handle(command, CancellationToken.None);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Sucesso.Should().BeFalse();
            resultado.Mensagem.Should().Contain("Já existe um cliente cadastrado com este CNPJ");
            resultado.Dados.Should().BeNull();

            _repositoryMock.Verify(
                x => x.AdicionarAsync(It.IsAny<Cliente>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ComCnpjInvalido_DeveRetornarErro()
        {
            // Arrange
            var command = new CriaClienteCommand("Empresa Teste LTDA", "11.222.333/0001-99");

            // Act
            var resultado = await _handler.Handle(command, CancellationToken.None);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Sucesso.Should().BeFalse();
            resultado.Mensagem.Should().Contain("CNPJ informado é inválido");
            resultado.Dados.Should().BeNull();

            _repositoryMock.Verify(
                x => x.AdicionarAsync(It.IsAny<Cliente>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ComNomeFantasiaVazio_DeveRetornarErro()
        {
            // Arrange
            var command = new CriaClienteCommand("", "11.222.333/0001-81");

            _repositoryMock
                .Setup(x => x.ObterPorCnpjAsync(It.IsAny<Cnpj>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Cliente?)null);

            // Act
            var resultado = await _handler.Handle(command, CancellationToken.None);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Sucesso.Should().BeFalse();
            resultado.Mensagem.Should().Contain("nome fantasia não pode ser vazio");
            resultado.Dados.Should().BeNull();

            _repositoryMock.Verify(
                x => x.AdicionarAsync(It.IsAny<Cliente>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ComNomeFantasiaMuitoCurto_DeveRetornarErro()
        {
            // Arrange
            var command = new CriaClienteCommand("AB", "11.222.333/0001-81");

            _repositoryMock
                .Setup(x => x.ObterPorCnpjAsync(It.IsAny<Cnpj>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Cliente?)null);

            // Act
            var resultado = await _handler.Handle(command, CancellationToken.None);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Sucesso.Should().BeFalse();
            resultado.Mensagem.Should().Contain("pelo menos 3 caracteres");
            resultado.Dados.Should().BeNull();

            _repositoryMock.Verify(
                x => x.AdicionarAsync(It.IsAny<Cliente>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_QuandoRepositorioLancaExcecao_DeveRetornarErro()
        {
            // Arrange
            var command = new CriaClienteCommand("Empresa Teste LTDA", "11.222.333/0001-81");

            _repositoryMock
                .Setup(x => x.ObterPorCnpjAsync(It.IsAny<Cnpj>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Cliente?)null);

            _repositoryMock
                .Setup(x => x.AdicionarAsync(It.IsAny<Cliente>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Erro no banco de dados"));

            // Act
            var resultado = await _handler.Handle(command, CancellationToken.None);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Sucesso.Should().BeFalse();
            resultado.Mensagem.Should().Contain("Erro ao criar cliente");
            resultado.Dados.Should().BeNull();
        }

        [Theory]
        [InlineData("Empresa A", "11.222.333/0001-81")]
        [InlineData("Empresa B", "34.028.316/0001-03")]
        [InlineData("Empresa C", "07.526.557/0001-00")]
        public async Task Handle_ComDiferentesCnpjsValidos_DeveCriarClientesComSucesso(
            string nomeFantasia,
            string cnpj)
        {
            // Arrange
            var command = new CriaClienteCommand(nomeFantasia, cnpj);

            _repositoryMock
                .Setup(x => x.ObterPorCnpjAsync(It.IsAny<Cnpj>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Cliente?)null);

            // Act
            var resultado = await _handler.Handle(command, CancellationToken.None);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Sucesso.Should().BeTrue();
            resultado.Dados.Should().NotBeNull();
            resultado.Dados!.NomeFantasia.Should().Be(nomeFantasia);
        }
    }
}
