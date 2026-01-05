using FluentAssertions;
using GestaoClientes.Domain.Exceptions;
using GestaoClientes.Domain.ValueObjects;

namespace GestaoClientes.Tests.Domain.ValueObjects;

public class CnpjTests
{
    [Theory]
    [InlineData("11.222.333/0001-81")]
    [InlineData("11222333000181")]
    public void Criar_ComCnpjValido_DeveCriarComSucesso(string cnpjValido)
    {
        var cnpj = Cnpj.Criar(cnpjValido);

        // Assert
        cnpj.Should().NotBeNull();
        cnpj.Valor.Should().HaveLength(14);
        cnpj.Valor.Should().MatchRegex("^[0-9]{14}$");
    }

    [Fact]
    public void Criar_ComCnpjVazio_DeveLancarExcecao()
    {
        // Arrange
        var cnpjVazio = "";

        // Act
        Action act = () => Cnpj.Criar(cnpjVazio);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("O CNPJ não pode ser vazio.");
    }

    [Fact]
    public void Criar_ComCnpjNulo_DeveLancarExcecao()
    {
        // Arrange
        string cnpjNulo = null!;

        // Act
        Action act = () => Cnpj.Criar(cnpjNulo);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("O CNPJ não pode ser vazio.");
    }

    [Theory]
    [InlineData("123")]
    [InlineData("11.222.333/0001")]
    [InlineData("112223330001811111")]
    public void Criar_ComCnpjTamanhoInvalido_DeveLancarExcecao(string cnpjInvalido)
    {
        // Act
        Action act = () => Cnpj.Criar(cnpjInvalido);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("O CNPJ deve conter 14 dígitos.");
    }

    [Theory]
    [InlineData("11111111111111")]
    [InlineData("00000000000000")]
    [InlineData("99999999999999")]
    public void Criar_ComTodosDigitosIguais_DeveLancarExcecao(string cnpjInvalido)
    {
        // Act
        Action act = () => Cnpj.Criar(cnpjInvalido);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("O CNPJ não pode ter todos os dígitos iguais.");
    }

    [Theory]
    [InlineData("11.222.333/0001-82")] // Dígito verificador inválido
    [InlineData("11222333000199")] // Dígito verificador inválido
    public void Criar_ComDigitoVerificadorInvalido_DeveLancarExcecao(string cnpjInvalido)
    {
        // Act
        Action act = () => Cnpj.Criar(cnpjInvalido);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("O CNPJ informado é inválido.");
    }

    [Fact]
    public void ObterFormatado_DeveRetornarCnpjFormatado()
    {
        // Arrange
        var cnpj = Cnpj.Criar("11222333000181");

        // Act
        var cnpjFormatado = cnpj.ObterFormatado();

        // Assert
        cnpjFormatado.Should().Be("11.222.333/0001-81");
    }

    [Fact]
    public void Equals_ComMesmoCnpj_DeveRetornarTrue()
    {
        // Arrange
        var cnpj1 = Cnpj.Criar("11.222.333/0001-81");
        var cnpj2 = Cnpj.Criar("11222333000181");

        // Act & Assert
        cnpj1.Should().Be(cnpj2);
        (cnpj1 == cnpj2).Should().BeTrue();
    }

    [Fact]
    public void Equals_ComCnpjDiferente_DeveRetornarFalse()
    {
        // Arrange
        var cnpj1 = Cnpj.Criar("11.222.333/0001-81");
        var cnpj2 = Cnpj.Criar("34.028.316/0001-03");

        // Act & Assert
        cnpj1.Should().NotBe(cnpj2);
        (cnpj1 != cnpj2).Should().BeTrue();
    }

    [Fact]
    public void ToString_DeveRetornarValorSemFormatacao()
    {
        // Arrange
        var cnpj = Cnpj.Criar("11.222.333/0001-81");

        // Act
        var resultado = cnpj.ToString();

        // Assert
        resultado.Should().Be("11222333000181");
    }
}
