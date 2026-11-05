namespace UneContIntegrationsTools.Core.Models;

/// <summary>
/// DTO com os parâmetros de configuração de conexão ao SQL Server.
/// </summary>
public sealed record SqlConnectionSettingsDto
{
    public string Server { get; init; } = string.Empty;
    public string Database { get; init; } = string.Empty;
    public SqlAuthenticationMode AuthenticationMode { get; init; } = SqlAuthenticationMode.SqlServerPassword;
    public string? Username { get; init; }
    public string? Password { get; init; }
    public bool Encrypt { get; init; } = true;
    public bool TrustServerCertificate { get; init; } = true;
    public int ConnectionTimeoutSeconds { get; init; } = 30;
}

