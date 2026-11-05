using System.ComponentModel;
using UneContIntegrationsTools.Features.Integracoes.Models;

namespace UneContIntegrationsTools.Features.Integracoes;

/// <summary>
/// UserControl (MVP View) para exibição da listagem da Fila de Integração.
/// </summary>
public partial class FilaIntegracaoControl : UserControl, IFilaIntegracaoView
{
    public FilaIntegracaoControl()
    {
        InitializeComponent();
        WireEvents();
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int NumeroTentativaMinimo
    {
        get => (int)_numMinAttempts.Value;
        set => _numMinAttempts.Value = Math.Clamp(value, (int)_numMinAttempts.Minimum, (int)_numMinAttempts.Maximum);
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string BuscaIntegracao
    {
        get => _txtSearch.Text;
        set => _txtSearch.Text = value;
    }

    public event Func<Task>? LoadRequested;
    public event Func<Task>? RefreshRequested;
    public event EventHandler? BackRequested;

    private void WireEvents()
    {
        Load += async (_, _) =>
        {
            if (LoadRequested is not null)
            {
                await LoadRequested.Invoke();
            }
        };

        _btnRefresh.Click += async (_, _) =>
        {
            if (RefreshRequested is not null)
            {
                await RefreshRequested.Invoke();
            }
        };

        _txtSearch.KeyDown += async (_, e) =>
        {
            if (e.KeyCode == Keys.Enter && RefreshRequested is not null)
            {
                e.SuppressKeyPress = true;
                await RefreshRequested.Invoke();
            }
        };

        _btnBack.Click += (_, e) => BackRequested?.Invoke(this, e);
    }

    public void SetBusyState(bool isBusy, string? statusMessage = null)
    {
        UseWaitCursor = isBusy;
        _btnRefresh.Enabled = !isBusy;
        _numMinAttempts.Enabled = !isBusy;
        _txtSearch.Enabled = !isBusy;

        if (!string.IsNullOrWhiteSpace(statusMessage))
        {
            _lblLastUpdatedValue.Text = statusMessage;
            _lblLastUpdatedValue.ForeColor = Color.FromArgb(37, 99, 235);
        }
    }

    public void ClearValidationErrors()
    {
        _errorProvider.Clear();
    }

    public void ShowValidationErrors(string errorMessage)
    {
        _errorProvider.SetError(_txtSearch, errorMessage);
        _lblLastUpdatedValue.Text = "Filtro inválido";
        _lblLastUpdatedValue.ForeColor = Color.FromArgb(180, 83, 9);
    }

    public void DisplayResult(FilaIntegracaoResumoDto resumo)
    {
        _dgvFila.DataSource = resumo.Itens;
        _lblTotalSistemasValue.Text = resumo.TotalSistemas.ToString("N0");
        _lblTotalNotasValue.Text = resumo.TotalNotas.ToString("N0");
        _lblLastUpdatedValue.Text = $"Atualizado às {resumo.ConsultadoEm:HH:mm:ss} ({resumo.ElapsedMilliseconds} ms)";
        _lblLastUpdatedValue.ForeColor = Color.FromArgb(21, 128, 61);
    }

    public void ShowError(string message)
    {
        _lblLastUpdatedValue.Text = "Falha na consulta";
        _lblLastUpdatedValue.ForeColor = Color.FromArgb(185, 28, 28);

        MessageBox.Show(
            this,
            message,
            "Fila de Integração - Atenção",
            MessageBoxButtons.OK,
            MessageBoxIcon.Warning);
    }
}

