#nullable enable
namespace UneContIntegrationsTools.UI;

partial class MainForm
{
    private System.ComponentModel.IContainer? components = null;
    private Panel _topBarPanel = null!;
    private Label _lblAppTitle = null!;
    private Label _lblActiveBreadcrumb = null!;
    private FlowLayoutPanel _topActionsPanel = null!;
    private Button _btnHome = null!;
    private Button _btnDatabaseConfig = null!;
    private Panel _contentHostPanel = null!;
    private Panel _statusBarPanel = null!;
    private Label _lblConnectionIndicator = null!;

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
        SuspendLayout();

        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1160, 740);
        MinimumSize = new Size(920, 620);
        StartPosition = FormStartPosition.CenterScreen;
        Text = "UneCont Integration Tools - Resolução de Tickets de Suporte";
        BackColor = Color.FromArgb(245, 247, 250);
        Font = new Font("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point);

        _topBarPanel = new Panel
        {
            Dock = DockStyle.Top,
            Height = 64,
            BackColor = Color.FromArgb(15, 23, 42),
            Padding = new Padding(24, 12, 24, 12)
        };

        _lblAppTitle = new Label
        {
            Text = "UneCont Integration Tools",
            Font = new Font("Segoe UI Semibold", 13F, FontStyle.Bold, GraphicsUnit.Point),
            ForeColor = Color.White,
            AutoSize = true,
            Location = new Point(24, 10)
        };

        _lblActiveBreadcrumb = new Label
        {
            Text = "Início  ›  Seleção de Funcionalidades",
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point),
            ForeColor = Color.FromArgb(148, 163, 184),
            AutoSize = true,
            Location = new Point(26, 36)
        };

        _topActionsPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Right,
            AutoSize = true,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            BackColor = Color.Transparent,
            Padding = new Padding(0, 4, 0, 0)
        };

        _btnHome = new Button
        {
            Text = "⊞ Tela Inicial (Funcionalidades)",
            Size = new Size(215, 34),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(30, 41, 59),
            ForeColor = Color.White,
            Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point),
            Cursor = Cursors.Hand,
            Margin = new Padding(0, 0, 10, 0)
        };
        _btnHome.FlatAppearance.BorderColor = Color.FromArgb(71, 85, 105);

        _btnDatabaseConfig = new Button
        {
            Text = "⚙ Conexão SQL / Entra ID",
            Size = new Size(195, 34),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(37, 99, 235),
            ForeColor = Color.White,
            Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point),
            Cursor = Cursors.Hand
        };
        _btnDatabaseConfig.FlatAppearance.BorderSize = 0;

        _topActionsPanel.Controls.Add(_btnHome);
        _topActionsPanel.Controls.Add(_btnDatabaseConfig);

        _topBarPanel.Controls.Add(_topActionsPanel);
        _topBarPanel.Controls.Add(_lblAppTitle);
        _topBarPanel.Controls.Add(_lblActiveBreadcrumb);

        _statusBarPanel = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 34,
            BackColor = Color.FromArgb(226, 232, 240),
            Padding = new Padding(20, 7, 20, 7)
        };

        _lblConnectionIndicator = new Label
        {
            Text = "● SQL Server: Não configurado",
            Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point),
            ForeColor = Color.FromArgb(180, 83, 9),
            AutoSize = true,
            Dock = DockStyle.Left
        };

        _statusBarPanel.Controls.Add(_lblConnectionIndicator);

        _contentHostPanel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(245, 247, 250)
        };

        Controls.Add(_contentHostPanel);
        Controls.Add(_statusBarPanel);
        Controls.Add(_topBarPanel);

        ResumeLayout(false);
    }
}
