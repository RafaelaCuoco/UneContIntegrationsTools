using UneContIntegrationsTools.Core.Interfaces;

namespace UneContIntegrationsTools.UI.Controls;

/// <summary>
/// UserControl da tela inicial com botões de seleção para cada funcionalidade/cenário cadastrado no sistema.
/// </summary>
public partial class HomeDashboardControl : UserControl
{
    public event EventHandler<IFeatureModule>? ModuleSelected;

    public HomeDashboardControl()
    {
        InitializeComponent();
    }

    public void LoadModules(IEnumerable<IFeatureModule> modules)
    {
        _buttonsContainer.SuspendLayout();
        _buttonsContainer.Controls.Clear();

        var orderedModules = modules.OrderBy(m => m.DisplayOrder).ThenBy(m => m.Title).ToList();

        if (orderedModules.Count == 0)
        {
            var emptyStateLabel = new Label
            {
                Text = "Nenhum cenário de suporte cadastrado no momento.\r\nOs botões das funcionalidades aparecerão aqui conforme forem adicionados.",
                Font = new Font("Segoe UI", 10.5F, FontStyle.Regular, GraphicsUnit.Point),
                ForeColor = Color.FromArgb(100, 116, 139),
                AutoSize = true,
                Margin = new Padding(4, 16, 0, 0)
            };
            _buttonsContainer.Controls.Add(emptyStateLabel);
        }
        else
        {
            foreach (var module in orderedModules)
            {
                var cardButton = CreateFeatureCard(module);
                _buttonsContainer.Controls.Add(cardButton);
            }
        }

        _buttonsContainer.ResumeLayout(true);
    }

    private Control CreateFeatureCard(IFeatureModule module)
    {
        var card = new Panel
        {
            Size = new Size(340, 165),
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle,
            Margin = new Padding(0, 0, 20, 20),
            Padding = new Padding(18, 16, 18, 16),
            Cursor = Cursors.Hand
        };

        var lblCategory = new Label
        {
            Text = module.Category.ToUpperInvariant(),
            Font = new Font("Segoe UI Semibold", 8F, FontStyle.Bold, GraphicsUnit.Point),
            ForeColor = Color.FromArgb(37, 99, 235),
            AutoSize = true,
            Location = new Point(18, 14),
            Cursor = Cursors.Hand
        };

        var lblTitle = new Label
        {
            Text = module.Title,
            Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point),
            ForeColor = Color.FromArgb(15, 23, 42),
            AutoSize = false,
            Size = new Size(300, 28),
            Location = new Point(18, 34),
            Cursor = Cursors.Hand
        };

        var lblDescription = new Label
        {
            Text = module.Description,
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point),
            ForeColor = Color.FromArgb(100, 116, 139),
            AutoSize = false,
            Size = new Size(300, 54),
            Location = new Point(18, 64),
            Cursor = Cursors.Hand
        };

        var btnOpen = new Button
        {
            Text = "Acessar Funcionalidade →",
            Size = new Size(302, 32),
            Location = new Point(18, 120),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(239, 246, 255),
            ForeColor = Color.FromArgb(29, 78, 216),
            Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point),
            Cursor = Cursors.Hand
        };
        btnOpen.FlatAppearance.BorderSize = 0;

        void TriggerSelection(object? s, EventArgs e) => ModuleSelected?.Invoke(this, module);

        card.Click += TriggerSelection;
        lblCategory.Click += TriggerSelection;
        lblTitle.Click += TriggerSelection;
        lblDescription.Click += TriggerSelection;
        btnOpen.Click += TriggerSelection;

        card.Controls.Add(lblCategory);
        card.Controls.Add(lblTitle);
        card.Controls.Add(lblDescription);
        card.Controls.Add(btnOpen);

        return card;
    }
}

