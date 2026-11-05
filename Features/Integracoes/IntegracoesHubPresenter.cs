using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace UneContIntegrationsTools.Features.Integracoes;

/// <summary>
/// Presenter (MVP) responsável pela navegação interna da tela de Integrações (ex.: Fila de Integração).
/// </summary>
public sealed class IntegracoesHubPresenter
{
    private readonly IIntegracoesHubView _view;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<IntegracoesHubPresenter> _logger;

    public IntegracoesHubPresenter(
        IIntegracoesHubView view,
        IServiceProvider serviceProvider,
        ILogger<IntegracoesHubPresenter> logger)
    {
        _view = view;
        _serviceProvider = serviceProvider;
        _logger = logger;

        _view.OpenFilaIntegracaoRequested += (_, _) => OpenFilaIntegracao();
    }

    private void OpenFilaIntegracao()
    {
        _logger.LogInformation("Abrindo subtela 'Fila de integração' dentro do módulo Integrações.");

        var filaControl = _serviceProvider.GetRequiredService<FilaIntegracaoControl>();
        _ = ActivatorUtilities.CreateInstance<FilaIntegracaoPresenter>(_serviceProvider, filaControl);

        filaControl.BackRequested += (_, _) =>
        {
            _logger.LogInformation("Retornando da 'Fila de integração' para o menu de Integrações.");
            _view.ShowMenu();
        };

        _view.ShowSubControl(filaControl);
    }
}

