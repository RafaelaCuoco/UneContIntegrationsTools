using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using UneContIntegrationsTools.Core.Interfaces;
using UneContIntegrationsTools.Core.Models;

namespace UneContIntegrationsTools.Infrastructure.Database;

/// <summary>
/// Persiste localmente as configurações de conexão SQL utilizando DPAPI (ProtectedData) do Windows para proteger a senha.
/// </summary>
public sealed class LocalConnectionSettingsStore(ILogger<LocalConnectionSettingsStore> logger) : IConnectionSettingsStore
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    private readonly string _settingsFilePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "UneContIntegrationsTools",
        "connection-settings.json");

    public async Task SaveAsync(SqlConnectionSettingsDto settings, bool rememberPassword, CancellationToken cancellationToken = default)
    {
        try
        {
            var directory = Path.GetDirectoryName(_settingsFilePath)!;
            Directory.CreateDirectory(directory);

            string? encryptedPassword = null;
            if (rememberPassword && !string.IsNullOrEmpty(settings.Password))
            {
                var plainBytes = Encoding.UTF8.GetBytes(settings.Password);
                var protectedBytes = ProtectedData.Protect(plainBytes, null, DataProtectionScope.CurrentUser);
                encryptedPassword = Convert.ToBase64String(protectedBytes);
            }

            var persisted = new PersistedSettingsModel(
                settings.Server,
                settings.Database,
                settings.AuthenticationMode,
                settings.Username,
                encryptedPassword,
                settings.Encrypt,
                settings.TrustServerCertificate,
                settings.ConnectionTimeoutSeconds);

            await using var stream = File.Create(_settingsFilePath);
            await JsonSerializer.SerializeAsync(stream, persisted, JsonOptions, cancellationToken);

            logger.LogInformation("Configurações de conexão salvas localmente em {FilePath}.", _settingsFilePath);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao salvar configurações de conexão locais.");
            throw;
        }
    }

    public async Task<SqlConnectionSettingsDto?> LoadAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (!File.Exists(_settingsFilePath))
            {
                return null;
            }

            await using var stream = File.OpenRead(_settingsFilePath);
            var persisted = await JsonSerializer.DeserializeAsync<PersistedSettingsModel>(stream, JsonOptions, cancellationToken);
            if (persisted is null)
            {
                return null;
            }

            string? decryptedPassword = null;
            if (!string.IsNullOrEmpty(persisted.EncryptedPassword))
            {
                try
                {
                    var protectedBytes = Convert.FromBase64String(persisted.EncryptedPassword);
                    var plainBytes = ProtectedData.Unprotect(protectedBytes, null, DataProtectionScope.CurrentUser);
                    decryptedPassword = Encoding.UTF8.GetString(plainBytes);
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Não foi possível descriptografar a senha salva. O usuário deverá informá-la novamente.");
                }
            }

            var authMode = Enum.IsDefined(persisted.AuthenticationMode)
                ? persisted.AuthenticationMode
                : SqlAuthenticationMode.SqlServerPassword;

            return new SqlConnectionSettingsDto
            {
                Server = persisted.Server,
                Database = persisted.Database,
                AuthenticationMode = authMode,
                Username = persisted.Username,
                Password = decryptedPassword,
                Encrypt = persisted.Encrypt,
                TrustServerCertificate = persisted.TrustServerCertificate,
                ConnectionTimeoutSeconds = persisted.ConnectionTimeoutSeconds
            };
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Não foi possível carregar as configurações de conexão salvas.");
            return null;
        }
    }

    private sealed record PersistedSettingsModel(
        string Server,
        string Database,
        SqlAuthenticationMode AuthenticationMode,
        string? Username,
        string? EncryptedPassword,
        bool Encrypt,
        bool TrustServerCertificate,
        int ConnectionTimeoutSeconds);
}

