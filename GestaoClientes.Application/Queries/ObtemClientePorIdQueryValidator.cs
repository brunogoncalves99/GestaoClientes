using FluentValidation;

namespace GestaoClientes.Application.Queries
{
    public class ObtemClientePorIdQueryValidator : AbstractValidator<ObtemClientePorIdQuery>
    {
        public ObtemClientePorIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("O ID do cliente é obrigatório.");
        }
    }
}
