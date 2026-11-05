using Microsoft.Data.SqlClient;
using UneContIntegrationsTools.Core.Models;

namespace UneContIntegrationsTools.Core.Interfaces;

/// <summary>
/// Fábrica e gerenciador de conexões com o SQL Server (suportando Login/Senha e Microsoft Entra ID).
/// </summary>
public interface ISqlConnectionFactory
{
    /// <summary>
    /// Evento disparado sempre que a configuração ativa de conexão for alterada.
    /// </summary>
    event EventHandler<SqlConnectionSettingsDto?>? ActiveSettingsChanged;

    /// <summary>
    /// Obtém a configuração de conexão atualmente ativa em memória.
    /// </summary>
    SqlConnectionSettingsDto? CurrentSettings { get; }

    /// <summary>
    /// Indica se existe uma configuração ativa definida.
    /// </summary>
    bool IsConfigured { get; }

    /// <summary>
    /// Define a configuração de conexão ativa que será utilizada pelos módulos do sistema.
    /// </summary>
    void SetActiveSettings(SqlConnectionSettingsDto settings);

    /// <summary>
    /// Constrói a ConnectionString segura a partir do DTO informado.
    /// </summary>
    string BuildConnectionString(SqlConnectionSettingsDto settings);

    /// <summary>
    /// Cria e abre de forma assíncrona uma nova conexão SQL utilizando a configuração ativa.
    /// </summary>
    Task<SqlConnection> CreateOpenConnectionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Cria e abre de forma assíncrona uma nova conexão SQL utilizando a configuração especificada.
    /// </summary>
    Task<SqlConnection> CreateOpenConnectionAsync(SqlConnectionSettingsDto settings, CancellationToken cancellationToken = default);

    /// <summary>
    /// Testa de forma assíncrona uma configuração de conexão e retorna informações do servidor/usuário autenticado.
    /// </summary>
    Task<ConnectionTestResult> TestConnectionAsync(SqlConnectionSettingsDto settings, CancellationToken cancellationToken = default);
}

