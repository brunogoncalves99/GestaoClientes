using FluentAssertions;
using GestaoClientes.Domain.Entities;
using GestaoClientes.Domain.Exceptions;
using GestaoClientes.Domain.ValueObjects;

namespace GestaoClientes.Tests.Domain.Entities;

public class ClienteTests
{
    [Fact]
    public void Criar_ComDadosValidos_DeveCriarClienteComSucesso()
    {
        // Arrange
        var nomeFantasia = "Empresa Teste LTDA";
        var cnpj = Cnpj.Criar("11.222.333/0001-81");

        // Act
        var cliente = Cliente.Criar(nomeFantasia, cnpj);

        // Assert
        cliente.Should().NotBeNull();
        cliente.Id.Should().NotBeEmpty();
        cliente.NomeFantasia.Should().Be(nomeFantasia);
        cliente.Cnpj.Should().Be(cnpj);
        cliente.Ativo.Should().BeTrue();
        cliente.DataCadastro.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        cliente.DataAtualizacao.Should().BeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData(null)]
    public void Criar_ComNomeFantasiaVazio_DeveLancarExcecao(string nomeInvalido)
    {
        // Arrange
        var cnpj = Cnpj.Criar("11.222.333/0001-81");

        // Act
        Action act = () => Cliente.Criar(nomeInvalido, cnpj);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("O nome fantasia não pode ser vazio.");
    }

    [Fact]
    public void Criar_ComNomeFantasiaMuitoCurto_DeveLancarExcecao()
    {
        // Arrange
        var nomeFantasia = "AB";
        var cnpj = Cnpj.Criar("11.222.333/0001-81");

        // Act
        Action act = () => Cliente.Criar(nomeFantasia, cnpj);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("O nome fantasia deve ter pelo menos 3 caracteres.");
    }

    [Fact]
    public void Criar_ComNomeFantasiaMuitoLongo_DeveLancarExcecao()
    {
        // Arrange
        var nomeFantasia = new string('A', 201);
        var cnpj = Cnpj.Criar("11.222.333/0001-81");

        // Act
        Action act = () => Cliente.Criar(nomeFantasia, cnpj);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("O nome fantasia não pode exceder 200 caracteres.");
    }

    [Fact]
    public void Criar_ComCnpjNulo_DeveLancarExcecao()
    {
        // Arrange
        var nomeFantasia = "Empresa Teste LTDA";

        // Act
        Action act = () => Cliente.Criar(nomeFantasia, null!);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("O CNPJ não pode ser nulo.");
    }

    [Fact]
    public void AtualizarNomeFantasia_ComNomeValido_DeveAtualizarComSucesso()
    {
        // Arrange
        var cliente = Cliente.Criar("Empresa Teste LTDA", Cnpj.Criar("11.222.333/0001-81"));
        var novoNome = "Empresa Atualizada LTDA";

        // Act
        cliente.AtualizarNomeFantasia(novoNome);

        // Assert
        cliente.NomeFantasia.Should().Be(novoNome);
        cliente.DataAtualizacao.Should().NotBeNull();
        cliente.DataAtualizacao.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void AtualizarNomeFantasia_ComNomeInvalido_DeveLancarExcecao()
    {
        // Arrange
        var cliente = Cliente.Criar("Empresa Teste LTDA", Cnpj.Criar("11.222.333/0001-81"));

        // Act
        Action act = () => cliente.AtualizarNomeFantasia("");

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("O nome fantasia não pode ser vazio.");
    }

    [Fact]
    public void Inativar_ComClienteAtivo_DeveInativarComSucesso()
    {
        // Arrange
        var cliente = Cliente.Criar("Empresa Teste LTDA", Cnpj.Criar("11.222.333/0001-81"));

        // Act
        cliente.Inativar();

        // Assert
        cliente.Ativo.Should().BeFalse();
        cliente.DataAtualizacao.Should().NotBeNull();
    }

    [Fact]
    public void Inativar_ComClienteJaInativo_DeveLancarExcecao()
    {
        // Arrange
        var cliente = Cliente.Criar("Empresa Teste LTDA", Cnpj.Criar("11.222.333/0001-81"));
        cliente.Inativar();

        // Act
        Action act = () => cliente.Inativar();

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("O cliente já está inativo.");
    }

    [Fact]
    public void Ativar_ComClienteInativo_DeveAtivarComSucesso()
    {
        // Arrange
        var cliente = Cliente.Criar("Empresa Teste LTDA", Cnpj.Criar("11.222.333/0001-81"));
        cliente.Inativar();

        // Act
        cliente.Ativar();

        // Assert
        cliente.Ativo.Should().BeTrue();
        cliente.DataAtualizacao.Should().NotBeNull();
    }

    [Fact]
    public void Ativar_ComClienteJaAtivo_DeveLancarExcecao()
    {
        // Arrange
        var cliente = Cliente.Criar("Empresa Teste LTDA", Cnpj.Criar("11.222.333/0001-81"));

        // Act
        Action act = () => cliente.Ativar();

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("O cliente já está ativo.");
    }

    [Fact]
    public void ToString_DeveRetornarRepresentacaoCorreta()
    {
        // Arrange
        var cliente = Cliente.Criar("Empresa Teste LTDA", Cnpj.Criar("11.222.333/0001-81"));

        // Act
        var resultado = cliente.ToString();

        // Assert
        resultado.Should().Be("Empresa Teste LTDA - 11.222.333/0001-81");
    }
}