using System.ComponentModel;
using UneContIntegrationsTools.Features.Integracoes.Models;

namespace UneContIntegrationsTools.Features.Integracoes;

/// <summary>
/// UserControl (MVP View) para exibição da listagem da Fila de Integração.
/// </summary>
public partial class FilaIntegracaoControl : UserControl, IFilaIntegracaoView
{
    private sealed record TipoServicoOption(TipoServicoFilaIntegracao Tipo, string DisplayText);

    private bool _suppressSelectionRefresh;

    public FilaIntegracaoControl()
    {
        InitializeComponent();
        InitializeTipoServicoOptions();
        WireEvents();
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public TipoServicoFilaIntegracao TipoServico
    {
        get => (_cmbTipoServico.SelectedItem as TipoServicoOption)?.Tipo ?? TipoServicoFilaIntegracao.ServicoTomado;
        set
        {
            _suppressSelectionRefresh = true;
            try
            {
                for (var i = 0; i < _cmbTipoServico.Items.Count; i++)
                {
                    if (_cmbTipoServico.Items[i] is TipoServicoOption option && option.Tipo == value)
                    {
                        _cmbTipoServico.SelectedIndex = i;
                        break;
                    }
                }

                UpdateSubtitle(value);
            }
            finally
            {
                _suppressSelectionRefresh = false;
            }
        }
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

    private void InitializeTipoServicoOptions()
    {
        _suppressSelectionRefresh = true;
        try
        {
            _cmbTipoServico.Items.AddRange(
            [
                new TipoServicoOption(TipoServicoFilaIntegracao.ServicoTomado, "Serviços Tomados"),
                new TipoServicoOption(TipoServicoFilaIntegracao.ServicoPrestado, "Serviços Prestados")
            ]);
            _cmbTipoServico.DisplayMember = nameof(TipoServicoOption.DisplayText);
            _cmbTipoServico.SelectedIndex = 0;
            UpdateSubtitle(TipoServicoFilaIntegracao.ServicoTomado);
        }
        finally
        {
            _suppressSelectionRefresh = false;
        }
    }

    private void UpdateSubtitle(TipoServicoFilaIntegracao tipoServico)
    {
        var label = tipoServico == TipoServicoFilaIntegracao.ServicoPrestado
            ? "Serviços Prestados"
            : "Serviços Tomados";

        _lblSubtitle.Text = $"Quantidade de notas fiscais ({label}) aguardando processamento por Sistema de Integração.";
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

        _cmbTipoServico.SelectedIndexChanged += async (_, _) =>
        {
            UpdateSubtitle(TipoServico);
            if (!_suppressSelectionRefresh && IsHandleCreated && RefreshRequested is not null)
            {
                await RefreshRequested.Invoke();
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
        _cmbTipoServico.Enabled = !isBusy;
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
        UpdateSubtitle(resumo.TipoServico);
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

