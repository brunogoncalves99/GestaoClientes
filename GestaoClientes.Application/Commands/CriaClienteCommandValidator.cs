using FluentValidation;

namespace GestaoClientes.Application.Commands;

public class CriaClienteCommandValidator : AbstractValidator<CriaClienteCommand>
{
    public CriaClienteCommandValidator()
    {
        RuleFor(x => x.NomeFantasia)
            .NotEmpty()
            .WithMessage("O nome fantasia é obrigatório.")
            .MinimumLength(3)
            .WithMessage("O nome fantasia deve ter pelo menos 3 caracteres.")
            .MaximumLength(200)
            .WithMessage("O nome fantasia não pode exceder 200 caracteres.");

        RuleFor(x => x.Cnpj)
            .NotEmpty()
            .WithMessage("O CNPJ é obrigatório.")
            .Must(cnpj => !string.IsNullOrWhiteSpace(cnpj) && cnpj.Replace(".", "").Replace("/", "").Replace("-", "").Length == 14)
            .WithMessage("O CNPJ deve conter 14 dígitos.");
    }
}