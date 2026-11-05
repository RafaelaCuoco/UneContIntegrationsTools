#nullable enable
namespace UneContIntegrationsTools.Features.Integracoes;

partial class IntegracoesHubControl
{
    private System.ComponentModel.IContainer? components = null;
    private Panel _menuContainerPanel = null!;
    private Panel _headerPanel = null!;
    private Label _lblTitle = null!;
    private Label _lblSubtitle = null!;
    private FlowLayoutPanel _cardsPanel = null!;
    private Panel _filaCard = null!;
    private Button _btnOpenFila = null!;

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

        _menuContainerPanel = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(28, 24, 28, 24),
            BackColor = Color.FromArgb(245, 247, 250)
        };

        _headerPanel = new Panel
        {
            Dock = DockStyle.Top,
            Height = 72,
            BackColor = Color.Transparent
        };

        _lblTitle = new Label
        {
            Text = "Integrações",
            Font = new Font("Segoe UI Semibold", 16F, FontStyle.Bold, GraphicsUnit.Point),
            ForeColor = Color.FromArgb(15, 23, 42),
            AutoSize = true,
            Location = new Point(0, 0)
        };

        _lblSubtitle = new Label
        {
            Text = "Selecione abaixo a operação ou consulta de integração que deseja realizar.",
            Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point),
            ForeColor = Color.FromArgb(100, 116, 139),
            AutoSize = true,
            Location = new Point(2, 36)
        };

        _headerPanel.Controls.Add(_lblTitle);
        _headerPanel.Controls.Add(_lblSubtitle);

        _cardsPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true,
            BackColor = Color.Transparent,
            Padding = new Padding(0, 8, 0, 8)
        };

        _filaCard = new Panel
        {
            Size = new Size(340, 165),
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle,
            Margin = new Padding(0, 0, 20, 20),
            Padding = new Padding(18, 16, 18, 16),
            Cursor = Cursors.Hand
        };

        var lblBadge = new Label
        {
            Text = "MONITORAMENTO NFE / SERVIÇO TOMADO",
            Font = new Font("Segoe UI Semibold", 8F, FontStyle.Bold, GraphicsUnit.Point),
            ForeColor = Color.FromArgb(37, 99, 235),
            AutoSize = true,
            Location = new Point(18, 14),
            Cursor = Cursors.Hand
        };

        var lblCardTitle = new Label
        {
            Text = "Fila de integração",
            Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point),
            ForeColor = Color.FromArgb(15, 23, 42),
            AutoSize = false,
            Size = new Size(300, 28),
            Location = new Point(18, 34),
            Cursor = Cursors.Hand
        };

        var lblCardDesc = new Label
        {
            Text = "Exibe a listagem agrupada de sistemas de integração e a quantidade de notas pendentes na fila.",
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point),
            ForeColor = Color.FromArgb(100, 116, 139),
            AutoSize = false,
            Size = new Size(300, 54),
            Location = new Point(18, 64),
            Cursor = Cursors.Hand
        };

        _btnOpenFila = new Button
        {
            Text = "Fila de integração →",
            Size = new Size(302, 32),
            Location = new Point(18, 120),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(37, 99, 235),
            ForeColor = Color.White,
            Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point),
            Cursor = Cursors.Hand
        };
        _btnOpenFila.FlatAppearance.BorderSize = 0;

        void TriggerOpenFila(object? s, EventArgs e) => OnFilaClicked(e);

        _filaCard.Click += TriggerOpenFila;
        lblBadge.Click += TriggerOpenFila;
        lblCardTitle.Click += TriggerOpenFila;
        lblCardDesc.Click += TriggerOpenFila;
        _btnOpenFila.Click += TriggerOpenFila;

        _filaCard.Controls.Add(lblBadge);
        _filaCard.Controls.Add(lblCardTitle);
        _filaCard.Controls.Add(lblCardDesc);
        _filaCard.Controls.Add(_btnOpenFila);

        _cardsPanel.Controls.Add(_filaCard);

        _menuContainerPanel.Controls.Add(_cardsPanel);
        _menuContainerPanel.Controls.Add(_headerPanel);

        Controls.Add(_menuContainerPanel);

        ResumeLayout(false);
    }
}

