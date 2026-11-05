namespace UneContIntegrationsTools.Core.Models;

/// <summary>
/// Modos de autenticação suportados para conexão com o SQL Server.
/// </summary>
public enum SqlAuthenticationMode
{
    /// <summary>
    /// Autenticação padrão do SQL Server (Usuário e Senha).
    /// </summary>
    SqlServerPassword = 0,

    /// <summary>
    /// Microsoft Entra ID Interativo (MFA / Login via navegador ou prompt da conta Microsoft).
    /// </summary>
    MicrosoftEntraInteractive = 1,

    /// <summary>
    /// Microsoft Entra ID com Usuário e Senha (ActiveDirectoryPassword).
    /// </summary>
    MicrosoftEntraPassword = 2,

    /// <summary>
    /// Microsoft Entra ID Integrado / Default (credenciais do ambiente/Windows/Azure CLI).
    /// </summary>
    MicrosoftEntraDefault = 3
}

