#nullable enable
namespace UneContIntegrationsTools.Features.Integracoes;

partial class FilaIntegracaoControl
{
    private System.ComponentModel.IContainer? components = null;
    private Panel _headerPanel = null!;
    private Button _btnBack = null!;
    private Label _lblTitle = null!;
    private Label _lblSubtitle = null!;
    private Panel _filterBarPanel = null!;
    private Label _lblTipoServico = null!;
    private ComboBox _cmbTipoServico = null!;
    private Label _lblMinAttempts = null!;
    private NumericUpDown _numMinAttempts = null!;
    private Label _lblSearch = null!;
    private TextBox _txtSearch = null!;
    private Button _btnRefresh = null!;
    private FlowLayoutPanel _kpiPanel = null!;
    private Label _lblTotalSistemasValue = null!;
    private Label _lblTotalNotasValue = null!;
    private Label _lblLastUpdatedValue = null!;
    private Panel _gridContainerPanel = null!;
    private DataGridView _dgvFila = null!;
    private DataGridViewTextBoxColumn _colIntegracao = null!;
    private DataGridViewTextBoxColumn _colQtdeNotas = null!;
    private ErrorProvider _errorProvider = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        _errorProvider = new ErrorProvider(components)
        {
            BlinkStyle = ErrorBlinkStyle.NeverBlink
        };

        SuspendLayout();

        BackColor = Color.FromArgb(245, 247, 250);
        Dock = DockStyle.Fill;
        Font = new Font("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point);
        Padding = new Padding(28, 20, 28, 20);

        _headerPanel = new Panel
        {
            Dock = DockStyle.Top,
            Height = 68,
            BackColor = Color.Transparent
        };

        _btnBack = new Button
        {
            Text = "← Voltar para Integrações",
            Size = new Size(195, 34),
            Location = new Point(0, 2),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.White,
            ForeColor = Color.FromArgb(30, 41, 59),
            Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point),
            Cursor = Cursors.Hand
        };
        _btnBack.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);

        _lblTitle = new Label
        {
            Text = "Fila de Integração",
            Font = new Font("Segoe UI Semibold", 15F, FontStyle.Bold, GraphicsUnit.Point),
            ForeColor = Color.FromArgb(15, 23, 42),
            AutoSize = true,
            Location = new Point(212, 0)
        };

        _lblSubtitle = new Label
        {
            Text = "Quantidade de notas fiscais (Serviços Tomados) aguardando processamento por Sistema de Integração.",
            Font = new Font("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point),
            ForeColor = Color.FromArgb(100, 116, 139),
            AutoSize = true,
            Location = new Point(214, 32)
        };

        _headerPanel.Controls.Add(_btnBack);
        _headerPanel.Controls.Add(_lblTitle);
        _headerPanel.Controls.Add(_lblSubtitle);

        _filterBarPanel = new Panel
        {
            Dock = DockStyle.Top,
            Height = 56,
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle,
            Padding = new Padding(16, 10, 16, 10)
        };

        _lblTipoServico = new Label
        {
            Text = "Tipo de Serviço:",
            Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point),
            ForeColor = Color.FromArgb(51, 65, 85),
            AutoSize = true,
            Location = new Point(16, 17)
        };

        _cmbTipoServico = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Width = 170,
            Location = new Point(124, 14)
        };

        _lblMinAttempts = new Label
        {
            Text = "Nº Tentativa Mínimo (>=):",
            Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point),
            ForeColor = Color.FromArgb(51, 65, 85),
            AutoSize = true,
            Location = new Point(312, 17)
        };

        _numMinAttempts = new NumericUpDown
        {
            Minimum = 0,
            Maximum = 9999,
            Value = 0,
            Width = 75,
            Location = new Point(478, 14)
        };

        _lblSearch = new Label
        {
            Text = "Filtrar Integração:",
            Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point),
            ForeColor = Color.FromArgb(51, 65, 85),
            AutoSize = true,
            Location = new Point(572, 17)
        };

        _txtSearch = new TextBox
        {
            Width = 220,
            PlaceholderText = "Ex.: Omie, Dominio, UneCont...",
            Location = new Point(690, 14)
        };

        _btnRefresh = new Button
        {
            Text = "↻ Atualizar Fila",
            Size = new Size(155, 32),
            Location = new Point(926, 11),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(37, 99, 235),
            ForeColor = Color.White,
            Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point),
            Cursor = Cursors.Hand
        };
        _btnRefresh.FlatAppearance.BorderSize = 0;

        _filterBarPanel.Controls.Add(_lblTipoServico);
        _filterBarPanel.Controls.Add(_cmbTipoServico);
        _filterBarPanel.Controls.Add(_lblMinAttempts);
        _filterBarPanel.Controls.Add(_numMinAttempts);
        _filterBarPanel.Controls.Add(_lblSearch);
        _filterBarPanel.Controls.Add(_txtSearch);
        _filterBarPanel.Controls.Add(_btnRefresh);

        _kpiPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 76,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            BackColor = Color.Transparent,
            Padding = new Padding(0, 10, 0, 10)
        };

        _kpiPanel.Controls.Add(CreateKpiCard("SISTEMAS NA FILA", out _lblTotalSistemasValue, "0"));
        _kpiPanel.Controls.Add(CreateKpiCard("TOTAL DE NOTAS (QTDE_NOTAS)", out _lblTotalNotasValue, "0"));
        _kpiPanel.Controls.Add(CreateKpiCard("STATUS DA CONSULTA", out _lblLastUpdatedValue, "Aguardando consulta...", width: 380));

        _gridContainerPanel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle,
            Padding = new Padding(1)
        };

        _dgvFila = new DataGridView
        {
            Dock = DockStyle.Fill,
            BackgroundColor = Color.White,
            BorderStyle = BorderStyle.None,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AllowUserToResizeRows = false,
            ReadOnly = true,
            MultiSelect = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AutoGenerateColumns = false,
            RowHeadersVisible = false,
            EnableHeadersVisualStyles = false,
            GridColor = Color.FromArgb(226, 232, 240),
            RowTemplate = { Height = 34 }
        };

        _dgvFila.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
        {
            BackColor = Color.FromArgb(241, 245, 249),
            ForeColor = Color.FromArgb(15, 23, 42),
            Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold, GraphicsUnit.Point),
            SelectionBackColor = Color.FromArgb(241, 245, 249),
            SelectionForeColor = Color.FromArgb(15, 23, 42),
            Padding = new Padding(8, 6, 8, 6)
        };
        _dgvFila.ColumnHeadersHeight = 38;
        _dgvFila.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

        _dgvFila.DefaultCellStyle = new DataGridViewCellStyle
        {
            BackColor = Color.White,
            ForeColor = Color.FromArgb(30, 41, 59),
            Font = new Font("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point),
            SelectionBackColor = Color.FromArgb(219, 234, 254),
            SelectionForeColor = Color.FromArgb(30, 58, 138),
            Padding = new Padding(8, 4, 8, 4)
        };

        _dgvFila.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
        {
            BackColor = Color.FromArgb(248, 250, 252)
        };

        _colIntegracao = new DataGridViewTextBoxColumn
        {
            Name = "INTEGRACAO",
            HeaderText = "INTEGRACAO (Sistema)",
            DataPropertyName = "Integracao",
            AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
            MinimumWidth = 240
        };

        _colQtdeNotas = new DataGridViewTextBoxColumn
        {
            Name = "QTDE_NOTAS",
            HeaderText = "QTDE_NOTAS",
            DataPropertyName = "QuantidadeNotas",
            Width = 220,
            DefaultCellStyle = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.MiddleRight,
                Format = "N0",
                Font = new Font("Consolas", 10F, FontStyle.Bold, GraphicsUnit.Point),
                Padding = new Padding(8, 4, 16, 4)
            },
            HeaderCell =
            {
                Style =
                {
                    Alignment = DataGridViewContentAlignment.MiddleRight
                }
            }
        };

        _dgvFila.Columns.AddRange(_colIntegracao, _colQtdeNotas);
        _gridContainerPanel.Controls.Add(_dgvFila);

        Controls.Add(_gridContainerPanel);
        Controls.Add(_kpiPanel);
        Controls.Add(_filterBarPanel);
        Controls.Add(_headerPanel);

        ResumeLayout(false);
    }

    private static Panel CreateKpiCard(string caption, out Label valueLabel, string initialValue, int width = 230)
    {
        var card = new Panel
        {
            Size = new Size(width, 56),
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle,
            Margin = new Padding(0, 0, 14, 0),
            Padding = new Padding(12, 6, 12, 6)
        };

        var lblCaption = new Label
        {
            Text = caption,
            Font = new Font("Segoe UI Semibold", 7.5F, FontStyle.Bold, GraphicsUnit.Point),
            ForeColor = Color.FromArgb(100, 116, 139),
            AutoSize = true,
            Location = new Point(12, 6)
        };

        valueLabel = new Label
        {
            Text = initialValue,
            Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold, GraphicsUnit.Point),
            ForeColor = Color.FromArgb(15, 23, 42),
            AutoSize = true,
            Location = new Point(12, 24)
        };

        card.Controls.Add(lblCaption);
        card.Controls.Add(valueLabel);
        return card;
    }
}

