using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using UneContIntegrationsTools.Core.Interfaces;
using UneContIntegrationsTools.Core.Models;
using UneContIntegrationsTools.Features.DatabaseConfig;

namespace UneContIntegrationsTools.UI;

/// <summary>
/// Presenter (MVP) da janela principal, gerenciando a navegação modular e o status global da conexão SQL Server.
/// </summary>
public sealed class MainPresenter
{
    private readonly IMainView _view;
    private readonly IServiceProvider _serviceProvider;
    private readonly IReadOnlyList<IFeatureModule> _modules;
    private readonly ISqlConnectionFactory _connectionFactory;
    private readonly IConnectionSettingsStore _settingsStore;
    private readonly IValidator<SqlConnectionSettingsDto> _validator;
    private readonly ILogger<MainPresenter> _logger;

    public MainPresenter(
        IMainView view,
        IServiceProvider serviceProvider,
        IEnumerable<IFeatureModule> modules,
        ISqlConnectionFactory connectionFactory,
        IConnectionSettingsStore settingsStore,
        IValidator<SqlConnectionSettingsDto> validator,
        ILogger<MainPresenter> logger)
    {
        _view = view;
        _serviceProvider = serviceProvider;
        _modules = modules.OrderBy(m => m.DisplayOrder).ThenBy(m => m.Title).ToList();
        _connectionFactory = connectionFactory;
        _settingsStore = settingsStore;
        _validator = validator;
        _logger = logger;

        _view.InitializeRequested += OnInitializeRequestedAsync;
        _view.NavigateHomeRequested += (_, _) => ShowHome();
        _view.OpenDatabaseConfigRequested += (_, _) => OpenDatabaseConfigScreen();
        _view.ModuleSelected += (_, module) => OpenModule(module);
        _connectionFactory.ActiveSettingsChanged += OnActiveSettingsChanged;
    }

    private async Task OnInitializeRequestedAsync()
    {
        try
        {
            var savedSettings = await _settingsStore.LoadAsync();
            if (savedSettings is not null)
            {
                var validation = await _validator.ValidateAsync(savedSettings);
                if (validation.IsValid)
                {
                    _connectionFactory.SetActiveSettings(savedSettings);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Não foi possível restaurar automaticamente a conexão SQL salva.");
        }

        RefreshConnectionStatus(_connectionFactory.CurrentSettings);
        ShowHome();
    }

    private void ShowHome()
    {
        _logger.LogInformation("Navegando para a tela inicial de seleção de funcionalidades.");
        _view.ShowHomeScreen(_modules);
    }

    private void OpenDatabaseConfigScreen()
    {
        _logger.LogInformation("Abrindo tela de configuração de conexão SQL Server / Entra ID.");
        var control = _serviceProvider.GetRequiredService<DatabaseConfigControl>();
        _ = ActivatorUtilities.CreateInstance<DatabaseConfigPresenter>(_serviceProvider, control);
        _view.ShowModuleControl("Conexão SQL Server / Entra ID", control);
    }

    private void OpenModule(IFeatureModule module)
    {
        _logger.LogInformation("Abrindo módulo {ModuleId} ({ModuleTitle}).", module.Id, module.Title);
        var control = module.CreateControl();
        _view.ShowModuleControl(module.Title, control);
    }

    private void OnActiveSettingsChanged(object? sender, SqlConnectionSettingsDto? settings)
    {
        RefreshConnectionStatus(settings);
    }

    private void RefreshConnectionStatus(SqlConnectionSettingsDto? settings)
    {
        if (settings is null)
        {
            _view.UpdateConnectionStatus(false, "SQL Server: Não configurado (Clique em 'Conexão SQL / Entra ID' para configurar)");
            return;
        }

        var authLabel = settings.AuthenticationMode switch
        {
            SqlAuthenticationMode.SqlServerPassword => $"SQL Login ({settings.Username})",
            SqlAuthenticationMode.MicrosoftEntraInteractive => "Microsoft Entra ID (Interativo / MFA)",
            SqlAuthenticationMode.MicrosoftEntraPassword => $"Microsoft Entra ID ({settings.Username})",
            SqlAuthenticationMode.MicrosoftEntraDefault => "Microsoft Entra ID (Default / Integrado)",
            _ => settings.AuthenticationMode.ToString()
        };

        _view.UpdateConnectionStatus(
            true,
            $"SQL Ativo: {settings.Server} | Banco: {settings.Database} | Autenticação: {authLabel}");
    }
}

