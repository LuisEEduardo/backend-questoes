using FluentValidation;
using Questao5.Application.Commands.Requests;

namespace Questao5.Application.Commands.Validadors
{
    public class CriarMovimentoCommandValidator : AbstractValidator<CriarMovimentoCommand>
    {
        public CriarMovimentoCommandValidator()
        {
            RuleFor(c => c.IdMovimento)
                .NotEmpty()
                .WithMessage("O {PropertyName} não pode ser vazio")
                .NotNull()
                .WithMessage("O {PropertyName} é obrigatório");

            RuleFor(c => c.IdContacorrente)
                .NotEmpty()
                .WithMessage("O {PropertyName} não pode ser vazio")
                .NotNull()
                .WithMessage("O {PropertyName} é obrigatório");

            RuleFor(c => c.Valor)                
                .GreaterThan(0)
                .WithMessage("TIPO: INVALID_VALUE | O {PropertyName} deve ser maior que 0");

            RuleFor(c => c.TipoMovimento)
                .Must(tipo => tipo.Equals('C') || tipo.Equals('D'))
                .WithMessage("TIPO: INVALID_TYPE | O {PropertyName} deve ser D (débito) ou C (crédito)");
        }
    }
}