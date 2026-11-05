using FluentValidation;
using Microsoft.Extensions.Logging;
using UneContIntegrationsTools.Core.Interfaces;
using UneContIntegrationsTools.Core.Models;

namespace UneContIntegrationsTools.Features.DatabaseConfig;

/// <summary>
/// Presenter (MVP) responsável pela lógica de validação, teste e ativação de conexões SQL Server / Microsoft Entra ID.
/// </summary>
public sealed class DatabaseConfigPresenter
{
    private readonly IDatabaseConfigView _view;
    private readonly ISqlConnectionFactory _connectionFactory;
    private readonly IConnectionSettingsStore _settingsStore;
    private readonly IValidator<SqlConnectionSettingsDto> _validator;
    private readonly ILogger<DatabaseConfigPresenter> _logger;

    public DatabaseConfigPresenter(
        IDatabaseConfigView view,
        ISqlConnectionFactory connectionFactory,
        IConnectionSettingsStore settingsStore,
        IValidator<SqlConnectionSettingsDto> validator,
        ILogger<DatabaseConfigPresenter> logger)
    {
        _view = view;
        _connectionFactory = connectionFactory;
        _settingsStore = settingsStore;
        _validator = validator;
        _logger = logger;

        _view.LoadRequested += OnLoadRequestedAsync;
        _view.TestConnectionRequested += OnTestConnectionRequestedAsync;
        _view.SaveAndActivateRequested += OnSaveAndActivateRequestedAsync;
    }

    private SqlConnectionSettingsDto ReadDtoFromView() => new()
    {
        Server = _view.Server.Trim(),
        Database = _view.Database.Trim(),
        AuthenticationMode = _view.AuthenticationMode,
        Username = _view.Username.Trim(),
        Password = _view.Password,
        Encrypt = _view.Encrypt,
        TrustServerCertificate = _view.TrustServerCertificate,
        ConnectionTimeoutSeconds = _view.ConnectionTimeoutSeconds
    };

    private void PopulateViewFromDto(SqlConnectionSettingsDto settings)
    {
        _view.Server = settings.Server;
        _view.Database = settings.Database;
        _view.AuthenticationMode = settings.AuthenticationMode;
        _view.Username = settings.Username ?? string.Empty;
        _view.Password = settings.Password ?? string.Empty;
        _view.Encrypt = settings.Encrypt;
        _view.TrustServerCertificate = settings.TrustServerCertificate;
        _view.ConnectionTimeoutSeconds = settings.ConnectionTimeoutSeconds;
        _view.RememberPassword = !string.IsNullOrEmpty(settings.Password);
    }

    private async Task OnLoadRequestedAsync()
    {
        try
        {
            var current = _connectionFactory.CurrentSettings ?? await _settingsStore.LoadAsync();
            if (current is not null)
            {
                PopulateViewFromDto(current);
                if (!_connectionFactory.IsConfigured)
                {
                    var validation = await _validator.ValidateAsync(current);
                    if (validation.IsValid)
                    {
                        _connectionFactory.SetActiveSettings(current);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Erro ao inicializar configurações de conexão na View.");
        }
    }

    private async Task<bool> ValidateViewAsync(SqlConnectionSettingsDto dto)
    {
        _view.ClearValidationErrors();
        var validationResult = await _validator.ValidateAsync(dto);

        if (validationResult.IsValid)
        {
            return true;
        }

        var errorDictionary = validationResult.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => string.Join(" ", g.Select(e => e.ErrorMessage)));

        _view.ShowValidationErrors(errorDictionary);
        return false;
    }

    private async Task OnTestConnectionRequestedAsync()
    {
        var dto = ReadDtoFromView();
        if (!await ValidateViewAsync(dto))
        {
            return;
        }

        try
        {
            var statusMsg = dto.AuthenticationMode == SqlAuthenticationMode.MicrosoftEntraInteractive
                ? "Aguardando autenticação Microsoft Entra ID (verifique a janela de login)..."
                : "Testando conexão com o SQL Server...";

            _view.SetBusyState(true, statusMsg);

            var result = await _connectionFactory.TestConnectionAsync(dto);
            _view.ShowConnectionResult(result);
        }
        finally
        {
            _view.SetBusyState(false);
        }
    }

    private async Task OnSaveAndActivateRequestedAsync()
    {
        var dto = ReadDtoFromView();
        if (!await ValidateViewAsync(dto))
        {
            return;
        }

        try
        {
            _view.SetBusyState(true, "Validando e ativando configuração de conexão...");

            var testResult = await _connectionFactory.TestConnectionAsync(dto);
            _view.ShowConnectionResult(testResult);

            if (!testResult.IsSuccess)
            {
                _view.ShowNotification(
                    "Não foi possível ativar a conexão porque o teste de conectividade falhou. Verifique os dados informados.",
                    isError: true);
                return;
            }

            _connectionFactory.SetActiveSettings(dto);
            await _settingsStore.SaveAsync(dto, _view.RememberPassword);

            _view.ShowNotification("Conexão SQL Server validada, ativada e salva com sucesso!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao salvar e ativar configuração de conexão SQL.");
            _view.ShowNotification($"Erro ao salvar configuração: {ex.Message}", isError: true);
        }
        finally
        {
            _view.SetBusyState(false);
        }
    }
}

