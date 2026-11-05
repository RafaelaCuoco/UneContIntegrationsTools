namespace UneContIntegrationsTools.Features.Integracoes;

/// <summary>
/// Contrato da View (MVP) para a tela de menu de Integrações.
/// </summary>
public interface IIntegracoesHubView
{
    event EventHandler? OpenFilaIntegracaoRequested;

    void ShowMenu();
    void ShowSubControl(UserControl subControl);
}

