using UneContIntegrationsTools.Features.Integracoes.Models;

namespace UneContIntegrationsTools.Features.Integracoes;

/// <summary>
/// Contrato da View (MVP) para a tela de Fila de Integração.
/// </summary>
public interface IFilaIntegracaoView
{
    TipoServicoFilaIntegracao TipoServico { get; set; }
    int NumeroTentativaMinimo { get; set; }
    string BuscaIntegracao { get; set; }

    event Func<Task>? LoadRequested;
    event Func<Task>? RefreshRequested;
    event EventHandler? BackRequested;

    void SetBusyState(bool isBusy, string? statusMessage = null);
    void ShowValidationErrors(string errorMessage);
    void ClearValidationErrors();
    void DisplayResult(FilaIntegracaoResumoDto resumo);
    void ShowError(string message);
}

