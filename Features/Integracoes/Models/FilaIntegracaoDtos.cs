namespace UneContIntegrationsTools.Features.Integracoes.Models;

/// <summary>
/// Parâmetros de filtro para consulta da Fila de Integração.
/// </summary>
public sealed record FilaIntegracaoFiltroDto(
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
    IReadOnlyList<FilaIntegracaoItemDto> Itens,
    int TotalSistemas,
    long TotalNotas,
    long ElapsedMilliseconds,
    DateTime ConsultadoEm);

