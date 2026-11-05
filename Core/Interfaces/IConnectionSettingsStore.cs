using UneContIntegrationsTools.Core.Models;

namespace UneContIntegrationsTools.Core.Interfaces;

/// <summary>
/// Contrato para persistência local segura das configurações de conexão SQL.
/// </summary>
public interface IConnectionSettingsStore
{
    /// <summary>
    /// Salva as configurações de conexão no armazenamento local do usuário.
    /// </summary>
    Task SaveAsync(SqlConnectionSettingsDto settings, bool rememberPassword, CancellationToken cancellationToken = default);

    /// <summary>
    /// Carrega as configurações de conexão salvas anteriormente, se existirem.
    /// </summary>
    Task<SqlConnectionSettingsDto?> LoadAsync(CancellationToken cancellationToken = default);
}

