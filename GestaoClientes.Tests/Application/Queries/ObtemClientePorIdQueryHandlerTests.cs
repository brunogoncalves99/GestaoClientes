using FluentAssertions;
using GestaoClientes.Application.Queries;
using GestaoClientes.Domain.Entities;
using GestaoClientes.Domain.Interfaces;
using GestaoClientes.Domain.ValueObjects;
using Moq;

namespace GestaoClientes.Tests.Application.Queries;

public class ObtemClientePorIdQueryHandlerTests
{
    private readonly Mock<IClienteRepository> _repositoryMock;
    private readonly ObtemClientePorIdQueryHandler _handler;

    public ObtemClientePorIdQueryHandlerTests()
    {
        _repositoryMock = new Mock<IClienteRepository>();
        _handler = new ObtemClientePorIdQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ComIdExistente_DeveRetornarClienteComSucesso()
    {
        // Arrange
        var clienteId = Guid.NewGuid();
        var cnpj = Cnpj.Criar("11.222.333/0001-81");
        var cliente = Cliente.Criar("Empresa Teste LTDA", cnpj);

        _repositoryMock
            .Setup(x => x.ObterPorIdAsync(clienteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cliente);

        var query = new ObtemClientePorIdQuery(clienteId);

        // Act
        var resultado = await _handler.Handle(query, CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Sucesso.Should().BeTrue();
        resultado.Mensagem.Should().Be("Cliente obtido com sucesso.");
        resultado.Dados.Should().NotBeNull();
        resultado.Dados!.NomeFantasia.Should().Be("Empresa Teste LTDA");
        resultado.Dados.Cnpj.Should().Be("11222333000181");
        resultado.Dados.CnpjFormatado.Should().Be("11.222.333/0001-81");
        resultado.Dados.Ativo.Should().BeTrue();

        _repositoryMock.Verify(
            x => x.ObterPorIdAsync(clienteId, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ComIdInexistente_DeveRetornarErroClienteNaoEncontrado()
    {
        // Arrange
        var clienteId = Guid.NewGuid();

        _repositoryMock
            .Setup(x => x.ObterPorIdAsync(clienteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Cliente?)null);

        var query = new ObtemClientePorIdQuery(clienteId);

        // Act
        var resultado = await _handler.Handle(query, CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Sucesso.Should().BeFalse();
        resultado.Mensagem.Should().Be("Cliente não encontrado.");
        resultado.Dados.Should().BeNull();

        _repositoryMock.Verify(
            x => x.ObterPorIdAsync(clienteId, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_QuandoRepositorioLancaExcecao_DeveRetornarErro()
    {
        // Arrange
        var clienteId = Guid.NewGuid();

        _repositoryMock
            .Setup(x => x.ObterPorIdAsync(clienteId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Erro no banco de dados"));

        var query = new ObtemClientePorIdQuery(clienteId);

        // Act
        var resultado = await _handler.Handle(query, CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Sucesso.Should().BeFalse();
        resultado.Mensagem.Should().Contain("Erro ao obter cliente");
        resultado.Dados.Should().BeNull();
    }

    [Fact]
    public async Task Handle_ComClienteInativo_DeveRetornarClienteComStatusCorreto()
    {
        // Arrange
        var clienteId = Guid.NewGuid();
        var cnpj = Cnpj.Criar("11.222.333/0001-81");
        var cliente = Cliente.Criar("Empresa Teste LTDA", cnpj);
        cliente.Inativar();

        _repositoryMock
            .Setup(x => x.ObterPorIdAsync(clienteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cliente);

        var query = new ObtemClientePorIdQuery(clienteId);

        // Act
        var resultado = await _handler.Handle(query, CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Sucesso.Should().BeTrue();
        resultado.Dados.Should().NotBeNull();
        resultado.Dados!.Ativo.Should().BeFalse();
        resultado.Dados.DataAtualizacao.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_ComClienteAtualizado_DeveRetornarDataAtualizacao()
    {
        // Arrange
        var clienteId = Guid.NewGuid();
        var cnpj = Cnpj.Criar("11.222.333/0001-81");
        var cliente = Cliente.Criar("Empresa Teste LTDA", cnpj);
        cliente.AtualizarNomeFantasia("Empresa Atualizada LTDA");

        _repositoryMock
            .Setup(x => x.ObterPorIdAsync(clienteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cliente);

        var query = new ObtemClientePorIdQuery(clienteId);

        // Act
        var resultado = await _handler.Handle(query, CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Sucesso.Should().BeTrue();
        resultado.Dados.Should().NotBeNull();
        resultado.Dados!.NomeFantasia.Should().Be("Empresa Atualizada LTDA");
        resultado.Dados.DataAtualizacao.Should().NotBeNull();
        resultado.Dados.DataAtualizacao.Should().BeAfter(resultado.Dados.DataCadastro);
    }

    [Theory]
    [InlineData("Empresa A")]
    [InlineData("Empresa B")]
    [InlineData("Empresa C")]
    public async Task Handle_ComDiferentesNomesFantasia_DeveRetornarCorreto(string nomeFantasia)
    {
        // Arrange
        var clienteId = Guid.NewGuid();
        var cnpj = Cnpj.Criar("11.222.333/0001-81");
        var cliente = Cliente.Criar(nomeFantasia, cnpj);

        _repositoryMock
            .Setup(x => x.ObterPorIdAsync(clienteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cliente);

        var query = new ObtemClientePorIdQuery(clienteId);

        // Act
        var resultado = await _handler.Handle(query, CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Sucesso.Should().BeTrue();
        resultado.Dados.Should().NotBeNull();
        resultado.Dados!.NomeFantasia.Should().Be(nomeFantasia);
    }
}
