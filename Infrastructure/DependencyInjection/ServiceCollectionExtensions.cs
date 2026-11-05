using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using UneContIntegrationsTools.Core.Interfaces;
using UneContIntegrationsTools.Core.Models;
using UneContIntegrationsTools.Core.Validators;
using UneContIntegrationsTools.Features.DatabaseConfig;
using UneContIntegrationsTools.Features.Integracoes;
using UneContIntegrationsTools.Features.Integracoes.Models;
using UneContIntegrationsTools.Features.Integracoes.Services;
using UneContIntegrationsTools.Features.Integracoes.Validators;
using UneContIntegrationsTools.Infrastructure.Database;
using UneContIntegrationsTools.UI;
using UneContIntegrationsTools.UI.Controls;

namespace UneContIntegrationsTools.Infrastructure.DependencyInjection;

/// <summary>
/// Registro centralizado de serviços, validadores, módulos e componentes visuais no container de DI.
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddLogging(builder => builder.AddSerilog(dispose: true));

        // Validadores (FluentValidation)
        services.AddSingleton<IValidator<SqlConnectionSettingsDto>, SqlConnectionSettingsValidator>();
        services.AddSingleton<IValidator<FilaIntegracaoFiltroDto>, FilaIntegracaoFiltroValidator>();

        // Infraestrutura de Banco de Dados (SQL Server & Microsoft Entra ID)
        services.AddSingleton<IConnectionSettingsStore, LocalConnectionSettingsStore>();
        services.AddSingleton<ISqlConnectionFactory, SqlConnectionFactory>();

        // Componentes da Janela Principal, Dashboard Inicial e Configuração SQL
        services.AddSingleton<HomeDashboardControl>();
        services.AddSingleton<MainForm>();
        services.AddSingleton<IMainView>(sp => sp.GetRequiredService<MainForm>());
        services.AddSingleton<MainPresenter>();
        services.AddTransient<DatabaseConfigControl>();

        // Módulos e Cenários de Integração (cada IFeatureModule gera um botão na tela inicial)
        services.AddTransient<IFilaIntegracaoService, FilaIntegracaoService>();
        services.AddTransient<FilaIntegracaoControl>();
        services.AddTransient<IntegracoesHubControl>();
        services.AddSingleton<IFeatureModule, IntegracoesModule>();

        return services;
    }
}

