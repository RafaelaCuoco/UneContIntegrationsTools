using UneContIntegrationsTools.Core.Models;

namespace UneContIntegrationsTools.Features.DatabaseConfig;

/// <summary>
/// Contrato da View (MVP) para configuração de conexão ao SQL Server e Microsoft Entra ID.
/// </summary>
public interface IDatabaseConfigView
{
    string Server { get; set; }
    string Database { get; set; }
    SqlAuthenticationMode AuthenticationMode { get; set; }
    string Username { get; set; }
    string Password { get; set; }
    bool Encrypt { get; set; }
    bool TrustServerCertificate { get; set; }
    int ConnectionTimeoutSeconds { get; set; }
    bool RememberPassword { get; set; }

    event Func<Task>? LoadRequested;
    event Func<Task>? TestConnectionRequested;
    event Func<Task>? SaveAndActivateRequested;

    void SetBusyState(bool isBusy, string? statusMessage = null);
    void ShowValidationErrors(IReadOnlyDictionary<string, string> propertyErrors);
    void ClearValidationErrors();
    void ShowConnectionResult(ConnectionTestResult result);
    void ShowNotification(string message, bool isError = false);
}

