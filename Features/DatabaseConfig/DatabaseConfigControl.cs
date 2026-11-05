using System.ComponentModel;
using UneContIntegrationsTools.Core.Models;

namespace UneContIntegrationsTools.Features.DatabaseConfig;

/// <summary>
/// UserControl independente (MVP View) para configuração e diagnóstico de conexão SQL Server / Microsoft Entra ID.
/// </summary>
public partial class DatabaseConfigControl : UserControl, IDatabaseConfigView
{
    private sealed record AuthModeOption(SqlAuthenticationMode Mode, string DisplayText);

    public DatabaseConfigControl()
    {
        InitializeComponent();
        InitializeAuthModeOptions();
        WireEvents();
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string Server
    {
        get => _txtServer.Text;
        set => _txtServer.Text = value;
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string Database
    {
        get => _txtDatabase.Text;
        set => _txtDatabase.Text = value;
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public SqlAuthenticationMode AuthenticationMode
    {
        get => (_cmbAuthMode.SelectedItem as AuthModeOption)?.Mode ?? SqlAuthenticationMode.SqlServerPassword;
        set
        {
            for (var i = 0; i < _cmbAuthMode.Items.Count; i++)
            {
                if (_cmbAuthMode.Items[i] is AuthModeOption option && option.Mode == value)
                {
                    _cmbAuthMode.SelectedIndex = i;
                    break;
                }
            }
        }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string Username
    {
        get => _txtUsername.Text;
        set => _txtUsername.Text = value;
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string Password
    {
        get => _txtPassword.Text;
        set => _txtPassword.Text = value;
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool Encrypt
    {
        get => _chkEncrypt.Checked;
        set => _chkEncrypt.Checked = value;
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool TrustServerCertificate
    {
        get => _chkTrustCert.Checked;
        set => _chkTrustCert.Checked = value;
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int ConnectionTimeoutSeconds
    {
        get => (int)_numTimeout.Value;
        set => _numTimeout.Value = Math.Clamp(value, (int)_numTimeout.Minimum, (int)_numTimeout.Maximum);
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RememberPassword
    {
        get => _chkRememberPassword.Checked;
        set => _chkRememberPassword.Checked = value;
    }

    public event Func<Task>? LoadRequested;
    public event Func<Task>? TestConnectionRequested;
    public event Func<Task>? SaveAndActivateRequested;

    private void InitializeAuthModeOptions()
    {
        _cmbAuthMode.Items.AddRange(
        [
            new AuthModeOption(SqlAuthenticationMode.SqlServerPassword, "SQL Server (Usuário e Senha)"),
            new AuthModeOption(SqlAuthenticationMode.MicrosoftEntraInteractive, "Microsoft Entra ID - Interativo (MFA / Navegador)"),
            new AuthModeOption(SqlAuthenticationMode.MicrosoftEntraPassword, "Microsoft Entra ID - Usuário e Senha"),
            new AuthModeOption(SqlAuthenticationMode.MicrosoftEntraDefault, "Microsoft Entra ID - Integrado / Default")
        ]);
        _cmbAuthMode.DisplayMember = nameof(AuthModeOption.DisplayText);
        _cmbAuthMode.SelectedIndex = 0;
    }

    private void WireEvents()
    {
        Load += async (_, _) =>
        {
            if (LoadRequested is not null)
            {
                await LoadRequested.Invoke();
            }
        };

        _btnTestConnection.Click += async (_, _) =>
        {
            if (TestConnectionRequested is not null)
            {
                await TestConnectionRequested.Invoke();
            }
        };

        _btnSaveActivate.Click += async (_, _) =>
        {
            if (SaveAndActivateRequested is not null)
            {
                await SaveAndActivateRequested.Invoke();
            }
        };
    }

    private void OnAuthModeChanged(object? sender, EventArgs e)
    {
        var mode = AuthenticationMode;
        var requiresPassword = mode is SqlAuthenticationMode.SqlServerPassword or SqlAuthenticationMode.MicrosoftEntraPassword;

        _txtPassword.Enabled = requiresPassword;
        _chkRememberPassword.Enabled = requiresPassword;

        if (!requiresPassword)
        {
            _errorProvider.SetError(_txtPassword, string.Empty);
        }

        _lblUsername.Text = mode switch
        {
            SqlAuthenticationMode.SqlServerPassword => "Usuário SQL:",
            SqlAuthenticationMode.MicrosoftEntraInteractive => "E-mail Entra (Opcional):",
            SqlAuthenticationMode.MicrosoftEntraPassword => "E-mail / Usuário Entra:",
            SqlAuthenticationMode.MicrosoftEntraDefault => "ClientId / Usuário (Opcional):",
            _ => "Usuário:"
        };

        _txtUsername.PlaceholderText = mode switch
        {
            SqlAuthenticationMode.SqlServerPassword => "Ex.: sa ou usr_integracao",
            SqlAuthenticationMode.MicrosoftEntraInteractive => "Opcional: seu.email@dominio.com.br (pré-seleciona a conta no MFA)",
            SqlAuthenticationMode.MicrosoftEntraPassword => "Ex.: seu.email@dominio.com.br",
            SqlAuthenticationMode.MicrosoftEntraDefault => "Deixe em branco para credencial padrão do ambiente",
            _ => string.Empty
        };
    }

    public void SetBusyState(bool isBusy, string? statusMessage = null)
    {
        UseWaitCursor = isBusy;
        _btnTestConnection.Enabled = !isBusy;
        _btnSaveActivate.Enabled = !isBusy;
        _formGrid.Enabled = !isBusy;

        if (!string.IsNullOrWhiteSpace(statusMessage))
        {
            _lblResultHeader.Text = "Processando...";
            _lblResultHeader.ForeColor = Color.FromArgb(37, 99, 235);
            _txtResultDetails.Text = statusMessage;
        }
    }

    public void ClearValidationErrors()
    {
        _errorProvider.Clear();
    }

    public void ShowValidationErrors(IReadOnlyDictionary<string, string> propertyErrors)
    {
        _errorProvider.Clear();

        foreach (var (property, message) in propertyErrors)
        {
            Control? targetControl = property switch
            {
                nameof(SqlConnectionSettingsDto.Server) => _txtServer,
                nameof(SqlConnectionSettingsDto.Database) => _txtDatabase,
                nameof(SqlConnectionSettingsDto.Username) => _txtUsername,
                nameof(SqlConnectionSettingsDto.Password) => _txtPassword,
                nameof(SqlConnectionSettingsDto.ConnectionTimeoutSeconds) => _numTimeout,
                _ => null
            };

            if (targetControl is not null)
            {
                _errorProvider.SetError(targetControl, message);
            }
        }

        _lblResultHeader.Text = "Atenção: Verifique os campos obrigatórios";
        _lblResultHeader.ForeColor = Color.FromArgb(180, 83, 9);
        _txtResultDetails.Text = string.Join(Environment.NewLine, propertyErrors.Values.Select(v => $"• {v}"));
    }

    public void ShowConnectionResult(ConnectionTestResult result)
    {
        if (result.IsSuccess)
        {
            _lblResultHeader.Text = $"✔ {result.Message} ({result.ElapsedMilliseconds} ms)";
            _lblResultHeader.ForeColor = Color.FromArgb(21, 128, 61);
            _txtResultDetails.Text =
                $"Servidor Respondido : {result.ServerName}{Environment.NewLine}" +
                $"Banco de Dados      : {result.DatabaseName}{Environment.NewLine}" +
                $"Usuário Autenticado : {result.AuthenticatedUser}{Environment.NewLine}" +
                $"Versão SQL Server   : {result.ServerVersion}{Environment.NewLine}" +
                $"Tempo de Resposta   : {result.ElapsedMilliseconds} ms";
        }
        else
        {
            _lblResultHeader.Text = $"✖ Falha na conexão ({result.ElapsedMilliseconds} ms)";
            _lblResultHeader.ForeColor = Color.FromArgb(185, 28, 28);
            _txtResultDetails.Text = result.Message;
        }
    }

    public void ShowNotification(string message, bool isError = false)
    {
        MessageBox.Show(
            this,
            message,
            isError ? "Erro de Configuração SQL" : "Conexão SQL Server",
            MessageBoxButtons.OK,
            isError ? MessageBoxIcon.Warning : MessageBoxIcon.Information);
    }
}
