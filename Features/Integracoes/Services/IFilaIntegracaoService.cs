using UneContIntegrationsTools.Features.Integracoes.Models;

namespace UneContIntegrationsTools.Features.Integracoes.Services;

/// <summary>
/// Contrato do serviço de consulta da Fila de Integração no SQL Server.
/// </summary>
public interface IFilaIntegracaoService
{
    Task<FilaIntegracaoResumoDto> ConsultarFilaAsync(
        FilaIntegracaoFiltroDto filtro,
        CancellationToken cancellationToken = default);
}

