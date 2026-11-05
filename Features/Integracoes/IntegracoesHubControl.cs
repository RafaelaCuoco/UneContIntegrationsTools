namespace UneContIntegrationsTools.Features.Integracoes;

/// <summary>
/// UserControl (MVP View) para a tela de Integrações, contendo o botão 'Fila de integração' e hospedando suas subtelas.
/// </summary>
public partial class IntegracoesHubControl : UserControl, IIntegracoesHubView
{
    private UserControl? _activeSubControl;

    public IntegracoesHubControl()
    {
        InitializeComponent();
    }

    public event EventHandler? OpenFilaIntegracaoRequested;

    private void OnFilaClicked(EventArgs e)
    {
        OpenFilaIntegracaoRequested?.Invoke(this, e);
    }

    public void ShowMenu()
    {
        SuspendLayout();

        if (_activeSubControl is not null)
        {
            Controls.Remove(_activeSubControl);
            _activeSubControl.Dispose();
            _activeSubControl = null;
        }

        _menuContainerPanel.Visible = true;
        ResumeLayout(true);
    }

    public void ShowSubControl(UserControl subControl)
    {
        SuspendLayout();

        if (_activeSubControl is not null)
        {
            Controls.Remove(_activeSubControl);
            _activeSubControl.Dispose();
        }

        _menuContainerPanel.Visible = false;
        _activeSubControl = subControl;
        _activeSubControl.Dock = DockStyle.Fill;
        Controls.Add(_activeSubControl);
        _activeSubControl.BringToFront();

        ResumeLayout(true);
    }
}

