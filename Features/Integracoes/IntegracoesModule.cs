using Microsoft.Extensions.DependencyInjection;
using UneContIntegrationsTools.Core.Interfaces;

namespace UneContIntegrationsTools.Features.Integracoes;

/// <summary>
/// Módulo principal de Integrações exibido como botão na tela inicial (HomeDashboardControl).
/// </summary>
public sealed class IntegracoesModule(IServiceProvider serviceProvider) : IFeatureModule
{
    public string Id => "integracao.hub";

    public string Title => "Integrações";

    public string Description => "Central de monitoramento e ferramentas de integração (Fila de integração e acompanhamento de notas).";

    public string Category => "Integrações";

    public int DisplayOrder => 10;

    public UserControl CreateControl()
    {
        var control = serviceProvider.GetRequiredService<IntegracoesHubControl>();
        _ = ActivatorUtilities.CreateInstance<IntegracoesHubPresenter>(serviceProvider, control);
        return control;
    }
}

