#nullable enable
namespace UneContIntegrationsTools.Features.DatabaseConfig;

partial class DatabaseConfigControl
{
    private System.ComponentModel.IContainer? components = null;

    private Panel _cardPanel = null!;
    private Label _lblTitle = null!;
    private Label _lblSubtitle = null!;
    private TableLayoutPanel _formGrid = null!;
    private Label _lblServer = null!;
    private TextBox _txtServer = null!;
    private Label _lblDatabase = null!;
    private TextBox _txtDatabase = null!;
    private Label _lblAuthMode = null!;
    private ComboBox _cmbAuthMode = null!;
    private Label _lblUsername = null!;
    private TextBox _txtUsername = null!;
    private Label _lblPassword = null!;
    private TextBox _txtPassword = null!;
    private FlowLayoutPanel _optionsPanel = null!;
    private CheckBox _chkEncrypt = null!;
    private CheckBox _chkTrustCert = null!;
    private CheckBox _chkRememberPassword = null!;
    private Label _lblTimeout = null!;
    private NumericUpDown _numTimeout = null!;
    private FlowLayoutPanel _actionsPanel = null!;
    private Button _btnTestConnection = null!;
    private Button _btnSaveActivate = null!;
    private Panel _resultPanel = null!;
    private Label _lblResultHeader = null!;
    private TextBox _txtResultDetails = null!;
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
        Padding = new Padding(28);
        AutoScroll = true;

        _cardPanel = new Panel
        {
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle,
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Padding = new Padding(28, 24, 28, 24)
        };

        var mainStack = new TableLayoutPanel
        {
            ColumnCount = 1,
            RowCount = 5,
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink
        };
        mainStack.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

        _lblTitle = new Label
        {
            Text = "Configuração de Conexão SQL Server & Microsoft Entra ID",
            Font = new Font("Segoe UI Semibold", 15F, FontStyle.Bold, GraphicsUnit.Point),
            ForeColor = Color.FromArgb(23, 37, 84),
            AutoSize = true,
            Margin = new Padding(0, 0, 0, 4)
        };

        _lblSubtitle = new Label
        {
            Text = "Configure o acesso ao banco de dados utilizando credenciais do SQL Server (Usuário/Senha) ou autenticação corporativa Microsoft Entra ID (MFA Interativo, Senha ou Integrado).",
            Font = new Font("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point),
            ForeColor = Color.FromArgb(100, 116, 139),
            AutoSize = true,
            Margin = new Padding(0, 0, 0, 20)
        };

        _formGrid = new TableLayoutPanel
        {
            ColumnCount = 2,
            RowCount = 7,
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Margin = new Padding(0, 0, 0, 16)
        };
        _formGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 220F));
        _formGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

        _lblServer = CreateFieldLabel("Servidor SQL (Host / Instância):");
        _txtServer = new TextBox
        {
            Width = 440,
            PlaceholderText = "Ex.: seu-servidor.database.windows.net ou localhost\\SQLEXPRESS",
            Margin = new Padding(0, 6, 24, 6)
        };

        _lblDatabase = CreateFieldLabel("Banco de Dados (Database):");
        _txtDatabase = new TextBox
        {
            Width = 440,
            PlaceholderText = "Ex.: UneContDb",
            Margin = new Padding(0, 6, 24, 6)
        };

        _lblAuthMode = CreateFieldLabel("Modo de Autenticação:");
        _cmbAuthMode = new ComboBox
        {
            Width = 440,
            DropDownStyle = ComboBoxStyle.DropDownList,
            Margin = new Padding(0, 6, 24, 6)
        };
        _cmbAuthMode.SelectedIndexChanged += OnAuthModeChanged;

        _lblUsername = CreateFieldLabel("Usuário / E-mail Entra:");
        _txtUsername = new TextBox
        {
            Width = 440,
            PlaceholderText = "Ex.: sa ou usuario@empresa.com.br",
            Margin = new Padding(0, 6, 24, 6)
        };

        _lblPassword = CreateFieldLabel("Senha:");
        _txtPassword = new TextBox
        {
            Width = 440,
            UseSystemPasswordChar = true,
            PlaceholderText = "Informe a senha de acesso",
            Margin = new Padding(0, 6, 24, 6)
        };

        _lblTimeout = CreateFieldLabel("Timeout de Conexão (s):");
        _numTimeout = new NumericUpDown
        {
            Minimum = 5,
            Maximum = 300,
            Value = 30,
            Width = 110,
            Margin = new Padding(0, 6, 0, 6)
        };

        _optionsPanel = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.LeftToRight,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Margin = new Padding(0, 8, 0, 8)
        };

        _chkEncrypt = new CheckBox
        {
            Text = "Criptografar conexão (Encrypt)",
            Checked = true,
            AutoSize = true,
            Margin = new Padding(0, 0, 20, 0)
        };

        _chkTrustCert = new CheckBox
        {
            Text = "Confiar no certificado do servidor (TrustServerCertificate)",
            Checked = true,
            AutoSize = true,
            Margin = new Padding(0, 0, 20, 0)
        };

        _chkRememberPassword = new CheckBox
        {
            Text = "Lembrar senha com segurança (Windows DPAPI)",
            Checked = false,
            AutoSize = true
        };

        _optionsPanel.Controls.Add(_chkEncrypt);
        _optionsPanel.Controls.Add(_chkTrustCert);
        _optionsPanel.Controls.Add(_chkRememberPassword);

        _formGrid.Controls.Add(_lblServer, 0, 0);
        _formGrid.Controls.Add(_txtServer, 1, 0);
        _formGrid.Controls.Add(_lblDatabase, 0, 1);
        _formGrid.Controls.Add(_txtDatabase, 1, 1);
        _formGrid.Controls.Add(_lblAuthMode, 0, 2);
        _formGrid.Controls.Add(_cmbAuthMode, 1, 2);
        _formGrid.Controls.Add(_lblUsername, 0, 3);
        _formGrid.Controls.Add(_txtUsername, 1, 3);
        _formGrid.Controls.Add(_lblPassword, 0, 4);
        _formGrid.Controls.Add(_txtPassword, 1, 4);
        _formGrid.Controls.Add(_lblTimeout, 0, 5);
        _formGrid.Controls.Add(_numTimeout, 1, 5);
        _formGrid.Controls.Add(CreateFieldLabel("Opções de Segurança:"), 0, 6);
        _formGrid.Controls.Add(_optionsPanel, 1, 6);

        _actionsPanel = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.LeftToRight,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Margin = new Padding(0, 8, 0, 18)
        };

        _btnTestConnection = new Button
        {
            Text = "Testar Conexão",
            Size = new Size(175, 40),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(241, 245, 249),
            ForeColor = Color.FromArgb(15, 23, 42),
            Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold, GraphicsUnit.Point),
            Cursor = Cursors.Hand,
            Margin = new Padding(0, 0, 12, 0)
        };
        _btnTestConnection.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);

        _btnSaveActivate = new Button
        {
            Text = "Salvar e Ativar Conexão",
            Size = new Size(215, 40),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(37, 99, 235),
            ForeColor = Color.White,
            Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold, GraphicsUnit.Point),
            Cursor = Cursors.Hand
        };
        _btnSaveActivate.FlatAppearance.BorderSize = 0;

        _actionsPanel.Controls.Add(_btnTestConnection);
        _actionsPanel.Controls.Add(_btnSaveActivate);

        _resultPanel = new Panel
        {
            Dock = DockStyle.Top,
            BackColor = Color.FromArgb(248, 250, 252),
            BorderStyle = BorderStyle.FixedSingle,
            Padding = new Padding(16),
            Height = 155
        };

        _lblResultHeader = new Label
        {
            Text = "Diagnóstico da Conexão",
            Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold, GraphicsUnit.Point),
            ForeColor = Color.FromArgb(51, 65, 85),
            Dock = DockStyle.Top,
            Height = 24
        };

        _txtResultDetails = new TextBox
        {
            Multiline = true,
            ReadOnly = true,
            BorderStyle = BorderStyle.None,
            BackColor = Color.FromArgb(248, 250, 252),
            ForeColor = Color.FromArgb(71, 85, 105),
            Font = new Font("Consolas", 9.5F, FontStyle.Regular, GraphicsUnit.Point),
            Dock = DockStyle.Fill,
            ScrollBars = ScrollBars.Vertical,
            Text = "Preencha os dados acima e clique em 'Testar Conexão' ou 'Salvar e Ativar Conexão'."
        };

        _resultPanel.Controls.Add(_txtResultDetails);
        _resultPanel.Controls.Add(_lblResultHeader);

        mainStack.Controls.Add(_lblTitle, 0, 0);
        mainStack.Controls.Add(_lblSubtitle, 0, 1);
        mainStack.Controls.Add(_formGrid, 0, 2);
        mainStack.Controls.Add(_actionsPanel, 0, 3);
        mainStack.Controls.Add(_resultPanel, 0, 4);

        _cardPanel.Controls.Add(mainStack);
        Controls.Add(_cardPanel);

        ResumeLayout(false);
        PerformLayout();
    }

    private static Label CreateFieldLabel(string text) => new()
    {
        Text = text,
        AutoSize = true,
        ForeColor = Color.FromArgb(51, 65, 85),
        Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold, GraphicsUnit.Point),
        Margin = new Padding(0, 9, 12, 6)
    };
}
