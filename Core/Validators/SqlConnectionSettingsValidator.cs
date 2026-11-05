using FluentValidation;
using UneContIntegrationsTools.Core.Models;

namespace UneContIntegrationsTools.Core.Validators;

/// <summary>
/// Validador FluentValidation para os parâmetros de conexão ao SQL Server e Microsoft Entra ID.
/// </summary>
public sealed class SqlConnectionSettingsValidator : AbstractValidator<SqlConnectionSettingsDto>
{
    public SqlConnectionSettingsValidator()
    {
        RuleFor(x => x.Server)
            .NotEmpty().WithMessage("O endereço do servidor SQL é obrigatório.")
            .MaximumLength(256).WithMessage("O endereço do servidor não pode exceder 256 caracteres.");

        RuleFor(x => x.Database)
            .NotEmpty().WithMessage("O nome do banco de dados é obrigatório.")
            .MaximumLength(128).WithMessage("O nome do banco de dados não pode exceder 128 caracteres.");

        RuleFor(x => x.ConnectionTimeoutSeconds)
            .InclusiveBetween(5, 300).WithMessage("O tempo limite de conexão deve estar entre 5 e 300 segundos.");

        When(x => x.AuthenticationMode is SqlAuthenticationMode.SqlServerPassword or SqlAuthenticationMode.MicrosoftEntraPassword, () =>
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("O usuário é obrigatório para o modo de autenticação selecionado.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("A senha é obrigatória para o modo de autenticação selecionado.");
        });
    }
}

