using FluentValidation;
using UneContIntegrationsTools.Features.Integracoes.Models;

namespace UneContIntegrationsTools.Features.Integracoes.Validators;

/// <summary>
/// Validador FluentValidation para os filtros da Fila de Integração.
/// </summary>
public sealed class FilaIntegracaoFiltroValidator : AbstractValidator<FilaIntegracaoFiltroDto>
{
    public FilaIntegracaoFiltroValidator()
    {
        RuleFor(x => x.TipoServico)
            .IsInEnum()
            .WithMessage("O tipo de serviço selecionado é inválido.");

        RuleFor(x => x.NumeroTentativaMinimo)
            .GreaterThanOrEqualTo(0)
            .WithMessage("O número mínimo de tentativas deve ser maior ou igual a zero.");

        RuleFor(x => x.BuscaIntegracao)
            .MaximumLength(100)
            .WithMessage("O filtro de nome da integração não pode exceder 100 caracteres.");
    }
}

