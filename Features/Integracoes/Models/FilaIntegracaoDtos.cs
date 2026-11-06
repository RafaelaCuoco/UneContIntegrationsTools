namespace UneContIntegrationsTools.Features.Integracoes.Models;

/// <summary>
/// Tipo de serviço fiscal consultado na Fila de Integração.
/// </summary>
public enum TipoServicoFilaIntegracao
{
    ServicoTomado = 0,
    ServicoPrestado = 1
}

/// <summary>
/// Parâmetros de filtro para consulta da Fila de Integração.
/// </summary>
public sealed record FilaIntegracaoFiltroDto(
    TipoServicoFilaIntegracao TipoServico = TipoServicoFilaIntegracao.ServicoTomado,
    int NumeroTentativaMinimo = 0,
    string? BuscaIntegracao = null);

/// <summary>
/// Item retornado pela consulta agrupada da Fila de Integração.
/// </summary>
public sealed record FilaIntegracaoItemDto(
    string Integracao,
    int QuantidadeNotas);

/// <summary>
/// Resultado consolidado da consulta da Fila de Integração.
/// </summary>
public sealed record FilaIntegracaoResumoDto(
    TipoServicoFilaIntegracao TipoServico,
    IReadOnlyList<FilaIntegracaoItemDto> Itens,
    int TotalSistemas,
    long TotalNotas,
    long ElapsedMilliseconds,
    DateTime ConsultadoEm);

