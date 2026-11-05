#nullable enable
namespace UneContIntegrationsTools.UI.Controls;

partial class HomeDashboardControl
{
    private System.ComponentModel.IContainer? components = null;
    private Panel _headerPanel = null!;
    private Label _lblWelcomeTitle = null!;
    private Label _lblWelcomeSubtitle = null!;
    private FlowLayoutPanel _buttonsContainer = null!;

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

        BackColor = Color.FromArgb(245, 247, 250);
        Dock = DockStyle.Fill;
        Font = new Font("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point);
        Padding = new Padding(28, 24, 28, 24);

        _headerPanel = new Panel
        {
            Dock = DockStyle.Top,
            Height = 76,
            BackColor = Color.Transparent
        };

        _lblWelcomeTitle = new Label
        {
            Text = "Painel de Funcionalidades e Resolução de Tickets",
            Font = new Font("Segoe UI Semibold", 16F, FontStyle.Bold, GraphicsUnit.Point),
            ForeColor = Color.FromArgb(15, 23, 42),
            AutoSize = true,
            Location = new Point(0, 0)
        };

        _lblWelcomeSubtitle = new Label
        {
            Text = "Selecione abaixo a ferramenta ou cenário de suporte que deseja executar.",
            Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point),
            ForeColor = Color.FromArgb(100, 116, 139),
            AutoSize = true,
            Location = new Point(2, 36)
        };

        _headerPanel.Controls.Add(_lblWelcomeTitle);
        _headerPanel.Controls.Add(_lblWelcomeSubtitle);

        _buttonsContainer = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true,
            BackColor = Color.Transparent,
            Padding = new Padding(0, 8, 0, 8)
        };

        Controls.Add(_buttonsContainer);
        Controls.Add(_headerPanel);

        ResumeLayout(false);
    }
}
