namespace UneContIntegrationsTools.Core.Models;

/// <summary>
/// Resultado estruturado do teste de conexão com o SQL Server.
/// </summary>
public sealed record ConnectionTestResult(
    bool IsSuccess,
    string Message,
    string? ServerName = null,
    string? DatabaseName = null,
    string? AuthenticatedUser = null,
    string? ServerVersion = null,
    long ElapsedMilliseconds = 0);

