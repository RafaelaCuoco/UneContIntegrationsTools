using System.Data;
using System.Diagnostics;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using UneContIntegrationsTools.Core.Interfaces;
using UneContIntegrationsTools.Features.Integracoes.Models;

namespace UneContIntegrationsTools.Features.Integracoes.Services;

/// <summary>
/// Serviço responsável por consultar a listagem agrupada da Fila de Integração no SQL Server.
/// </summary>
public sealed class FilaIntegracaoService(
    ISqlConnectionFactory connectionFactory,
    ILogger<FilaIntegracaoService> logger) : IFilaIntegracaoService
{
    private const string BaseQuerySql = """
        SELECT DISTINCT SEI.INTEGRACAO, COUNT(ST.ServicoTomadoId) AS QTDE_NOTAS
        FROM Parceiros.ParceiroEmpresa PE
        INNER JOIN NFe.ServicoTomado ST ON ST.ParceiroEmpresaId = PE.ParceiroEmpresaId
        INNER JOIN Parceiros.ParceiroEmpresaConfiguracaoIntegracao PECI ON PECI.ParceiroEmpresaId = PE.ParceiroEmpresaID
        INNER JOIN (
            SELECT DISTINCT SistemaIntegracaoId,
                (CASE WHEN SistemaIntegracaoId = 0  THEN 'NAO INFORMADO'
                      WHEN SistemaIntegracaoId = 1  THEN 'Vianuvem'
                      WHEN SistemaIntegracaoId = 2  THEN 'SCIWeb'
                      WHEN SistemaIntegracaoId = 3  THEN 'WebHook'
                      WHEN SistemaIntegracaoId = 4  THEN 'UneCont'
                      WHEN SistemaIntegracaoId = 5  THEN 'NetSuiteActive'
                      WHEN SistemaIntegracaoId = 6  THEN 'Email'
                      WHEN SistemaIntegracaoId = 7  THEN 'Talkcom'
                      WHEN SistemaIntegracaoId = 8  THEN 'Omie'
                      WHEN SistemaIntegracaoId = 9  THEN 'SFTP'
                      WHEN SistemaIntegracaoId = 10 THEN 'Holmes'
                      WHEN SistemaIntegracaoId = 11 THEN 'WebHookUneCont'
                      WHEN SistemaIntegracaoId = 12 THEN 'RMWalm'
                      WHEN SistemaIntegracaoId = 13 THEN 'Questor'
                      WHEN SistemaIntegracaoId = 14 THEN 'Dominio'
                      WHEN SistemaIntegracaoId = 15 THEN 'WKRadar'
                      WHEN SistemaIntegracaoId = 16 THEN 'RM Hausen'
                      ELSE NULL END) AS INTEGRACAO
            FROM Parceiros.ParceiroEmpresaConfiguracaoIntegracao
        ) AS SEI ON SEI.SistemaIntegracaoId = PECI.SistemaIntegracaoId
        INNER JOIN NFe.FilaIntegracaoSistema FILA ON FILA.IdNota = ST.ServicoTomadoId
        WHERE FILA.NumeroTentativa >= @NumeroTentativaMinimo
        GROUP BY SEI.INTEGRACAO
        ORDER BY COUNT(ST.ServicoTomadoId) DESC;
        """;

    public async Task<FilaIntegracaoResumoDto> ConsultarFilaAsync(
        FilaIntegracaoFiltroDto filtro,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(filtro);

        var stopwatch = Stopwatch.StartNew();

        logger.LogInformation(
            "Iniciando consulta da Fila de Integração (NumeroTentativa >= {NumeroTentativaMinimo}, Busca: '{Busca}').",
            filtro.NumeroTentativaMinimo,
            filtro.BuscaIntegracao ?? string.Empty);

        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = BaseQuerySql;
        command.CommandTimeout = connectionFactory.CurrentSettings?.ConnectionTimeoutSeconds ?? 60;
        command.Parameters.Add(new SqlParameter("@NumeroTentativaMinimo", SqlDbType.Int)
        {
            Value = filtro.NumeroTentativaMinimo
        });

        var items = new List<FilaIntegracaoItemDto>();

        await using (var reader = await command.ExecuteReaderAsync(cancellationToken))
        {
            while (await reader.ReadAsync(cancellationToken))
            {
                var integracao = reader.IsDBNull(0) ? "NÃO IDENTIFICADO" : reader.GetString(0);
                var qtdeNotas = reader.IsDBNull(1) ? 0 : Convert.ToInt32(reader.GetValue(1));

                items.Add(new FilaIntegracaoItemDto(integracao, qtdeNotas));
            }
        }

        if (!string.IsNullOrWhiteSpace(filtro.BuscaIntegracao))
        {
            var termo = filtro.BuscaIntegracao.Trim();
            items = items
                .Where(i => i.Integracao.Contains(termo, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        stopwatch.Stop();

        var totalNotas = items.Sum(i => (long)i.QuantidadeNotas);

        logger.LogInformation(
            "Consulta da Fila de Integração concluída em {ElapsedMs} ms. Sistemas retornados: {Count}, Total de notas: {TotalNotas}.",
            stopwatch.ElapsedMilliseconds,
            items.Count,
            totalNotas);

        return new FilaIntegracaoResumoDto(
            Itens: items,
            TotalSistemas: items.Count,
            TotalNotas: totalNotas,
            ElapsedMilliseconds: stopwatch.ElapsedMilliseconds,
            ConsultadoEm: DateTime.Now);
    }
}

