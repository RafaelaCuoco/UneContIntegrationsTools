using System.Diagnostics;
using FluentValidation;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using UneContIntegrationsTools.Core.Interfaces;
using UneContIntegrationsTools.Core.Models;

namespace UneContIntegrationsTools.Infrastructure.Database;

/// <summary>
/// Implementação da fábrica de conexões SQL Server com suporte a SQL Login e Microsoft Entra ID.
/// </summary>
public sealed class SqlConnectionFactory(
    IValidator<SqlConnectionSettingsDto> validator,
    ILogger<SqlConnectionFactory> logger) : ISqlConnectionFactory
{
    private readonly Lock _syncLock = new();
    private SqlConnectionSettingsDto? _currentSettings;

    public event EventHandler<SqlConnectionSettingsDto?>? ActiveSettingsChanged;

    public SqlConnectionSettingsDto? CurrentSettings
    {
        get
        {
            lock (_syncLock)
            {
                return _currentSettings;
            }
        }
    }

    public bool IsConfigured => CurrentSettings is not null;

    public void SetActiveSettings(SqlConnectionSettingsDto settings)
    {
        ArgumentNullException.ThrowIfNull(settings);

        var validationResult = validator.Validate(settings);
        if (!validationResult.IsValid)
        {
            var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            throw new ValidationException($"Configuração de conexão inválida: {errors}", validationResult.Errors);
        }

        lock (_syncLock)
        {
            _currentSettings = settings;
        }

        logger.LogInformation(
            "Configuração ativa de conexão SQL atualizada. Servidor: {Server}, Banco: {Database}, Modo: {AuthMode}",
            settings.Server,
            settings.Database,
            settings.AuthenticationMode);

        ActiveSettingsChanged?.Invoke(this, settings);
    }

    public string BuildConnectionString(SqlConnectionSettingsDto settings)
    {
        ArgumentNullException.ThrowIfNull(settings);

        var builder = new SqlConnectionStringBuilder
        {
            DataSource = settings.Server.Trim(),
            InitialCatalog = settings.Database.Trim(),
            Encrypt = settings.Encrypt,
            TrustServerCertificate = settings.TrustServerCertificate,
            ConnectTimeout = settings.ConnectionTimeoutSeconds,
            ApplicationName = "UneContIntegrationTools"
        };

        switch (settings.AuthenticationMode)
        {
            case SqlAuthenticationMode.SqlServerPassword:
                builder.IntegratedSecurity = false;
                builder.UserID = settings.Username?.Trim() ?? string.Empty;
                builder.Password = settings.Password ?? string.Empty;
                break;

            case SqlAuthenticationMode.MicrosoftEntraInteractive:
                builder.Authentication = SqlAuthenticationMethod.ActiveDirectoryInteractive;
                if (!string.IsNullOrWhiteSpace(settings.Username))
                {
                    builder.UserID = settings.Username.Trim();
                }
                break;

            case SqlAuthenticationMode.MicrosoftEntraPassword:
#pragma warning disable CS0618
                builder.Authentication = SqlAuthenticationMethod.ActiveDirectoryPassword;
#pragma warning restore CS0618
                builder.UserID = settings.Username?.Trim() ?? string.Empty;
                builder.Password = settings.Password ?? string.Empty;
                break;

            case SqlAuthenticationMode.MicrosoftEntraDefault:
                builder.Authentication = SqlAuthenticationMethod.ActiveDirectoryDefault;
                if (!string.IsNullOrWhiteSpace(settings.Username))
                {
                    builder.UserID = settings.Username.Trim();
                }
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(settings), settings.AuthenticationMode, "Modo de autenticação não suportado.");
        }

        return builder.ConnectionString;
    }

    public Task<SqlConnection> CreateOpenConnectionAsync(CancellationToken cancellationToken = default)
    {
        var active = CurrentSettings
            ?? throw new InvalidOperationException("Nenhuma conexão SQL Server foi configurada. Configure a conexão antes de executar esta operação.");

        return CreateOpenConnectionAsync(active, cancellationToken);
    }

    public async Task<SqlConnection> CreateOpenConnectionAsync(SqlConnectionSettingsDto settings, CancellationToken cancellationToken = default)
    {
        var validationResult = await validator.ValidateAsync(settings, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var connectionString = BuildConnectionString(settings);
        var connection = new SqlConnection(connectionString);

        try
        {
            await connection.OpenAsync(cancellationToken);
            return connection;
        }
        catch
        {
            await connection.DisposeAsync();
            throw;
        }
    }

    public async Task<ConnectionTestResult> TestConnectionAsync(SqlConnectionSettingsDto settings, CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            logger.LogInformation(
                "Iniciando teste de conexão SQL. Servidor: {Server}, Banco: {Database}, Autenticação: {AuthMode}",
                settings.Server,
                settings.Database,
                settings.AuthenticationMode);

            await using var connection = await CreateOpenConnectionAsync(settings, cancellationToken);
            await using var command = connection.CreateCommand();
            command.CommandText = "SELECT @@SERVERNAME AS ServerName, DB_NAME() AS DatabaseName, SYSTEM_USER AS AuthenticatedUser;";
            command.CommandTimeout = settings.ConnectionTimeoutSeconds;

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            string? serverName = connection.DataSource;
            string? dbName = connection.Database;
            string? authenticatedUser = null;

            if (await reader.ReadAsync(cancellationToken))
            {
                serverName = reader.IsDBNull(0) ? connection.DataSource : reader.GetString(0);
                dbName = reader.IsDBNull(1) ? connection.Database : reader.GetString(1);
                authenticatedUser = reader.IsDBNull(2) ? null : reader.GetString(2);
            }

            stopwatch.Stop();

            logger.LogInformation(
                "Conexão SQL testada com sucesso em {ElapsedMs} ms. Servidor: {ServerName}, Banco: {DatabaseName}, Usuário: {User}",
                stopwatch.ElapsedMilliseconds,
                serverName,
                dbName,
                authenticatedUser);

            return new ConnectionTestResult(
                IsSuccess: true,
                Message: "Conexão estabelecida com sucesso!",
                ServerName: serverName,
                DatabaseName: dbName,
                AuthenticatedUser: authenticatedUser,
                ServerVersion: connection.ServerVersion,
                ElapsedMilliseconds: stopwatch.ElapsedMilliseconds);
        }
        catch (ValidationException vex)
        {
            stopwatch.Stop();
            var errorMsg = string.Join(Environment.NewLine, vex.Errors.Select(e => $"• {e.ErrorMessage}"));
            logger.LogWarning("Falha de validação ao testar conexão SQL: {Errors}", errorMsg);

            return new ConnectionTestResult(
                IsSuccess: false,
                Message: errorMsg,
                ElapsedMilliseconds: stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            logger.LogError(
                ex,
                "Erro ao conectar ao SQL Server ({Server} / {Database}) usando {AuthMode} após {ElapsedMs} ms.",
                settings.Server,
                settings.Database,
                settings.AuthenticationMode,
                stopwatch.ElapsedMilliseconds);

            return new ConnectionTestResult(
                IsSuccess: false,
                Message: ex.Message,
                ElapsedMilliseconds: stopwatch.ElapsedMilliseconds);
        }
    }
}
