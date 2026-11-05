using UneContIntegrationsTools.Core.Interfaces;
using UneContIntegrationsTools.UI.Controls;

namespace UneContIntegrationsTools.UI;

/// <summary>
/// Janela principal do sistema UneContIntegrationTools, implementando IMainView (MVP).
/// </summary>
public partial class MainForm : Form, IMainView
{
    private readonly HomeDashboardControl _homeDashboardControl;

    public MainForm(HomeDashboardControl homeDashboardControl)
    {
        _homeDashboardControl = homeDashboardControl;
        InitializeComponent();
        WireEvents();
    }

    public event Func<Task>? InitializeRequested;
    public event EventHandler? NavigateHomeRequested;
    public event EventHandler? OpenDatabaseConfigRequested;
    public event EventHandler<IFeatureModule>? ModuleSelected;

    private void WireEvents()
    {
        Load += async (_, _) =>
        {
            if (InitializeRequested is not null)
            {
                await InitializeRequested.Invoke();
            }
        };

        _btnHome.Click += (_, e) => NavigateHomeRequested?.Invoke(this, e);
        _btnDatabaseConfig.Click += (_, e) => OpenDatabaseConfigRequested?.Invoke(this, e);
        _homeDashboardControl.ModuleSelected += (_, module) => ModuleSelected?.Invoke(this, module);
    }

    public void ShowHomeScreen(IReadOnlyList<IFeatureModule> modules)
    {
        _lblActiveBreadcrumb.Text = "Início  ›  Seleção de Funcionalidades";
        _homeDashboardControl.LoadModules(modules);
        SwapActiveControl(_homeDashboardControl, disposePreviousIfNotHome: true);
    }

    public void ShowModuleControl(string moduleTitle, UserControl control)
    {
        _lblActiveBreadcrumb.Text = $"Início  ›  {moduleTitle}";
        SwapActiveControl(control, disposePreviousIfNotHome: true);
    }

    public void UpdateConnectionStatus(bool isConfigured, string statusText)
    {
        if (InvokeRequired)
        {
            Invoke(() => UpdateConnectionStatus(isConfigured, statusText));
            return;
        }

        _lblConnectionIndicator.Text = $"● {statusText}";
        _lblConnectionIndicator.ForeColor = isConfigured
            ? Color.FromArgb(21, 128, 61)
            : Color.FromArgb(180, 83, 9);
    }

    private void SwapActiveControl(UserControl nextControl, bool disposePreviousIfNotHome)
    {
        _contentHostPanel.SuspendLayout();

        var existingControls = _contentHostPanel.Controls.Cast<Control>().ToList();
        _contentHostPanel.Controls.Clear();

        if (disposePreviousIfNotHome)
        {
            foreach (var oldControl in existingControls)
            {
                if (!ReferenceEquals(oldControl, _homeDashboardControl) && !ReferenceEquals(oldControl, nextControl))
                {
                    oldControl.Dispose();
                }
            }
        }

        nextControl.Dock = DockStyle.Fill;
        _contentHostPanel.Controls.Add(nextControl);
        _contentHostPanel.ResumeLayout(true);
    }
}

