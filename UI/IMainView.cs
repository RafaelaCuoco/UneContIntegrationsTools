using UneContIntegrationsTools.Core.Interfaces;

namespace UneContIntegrationsTools.UI;

/// <summary>
/// Contrato da View principal (MVP) que hospeda a tela inicial de seleção de funcionalidades e os módulos.
/// </summary>
public interface IMainView
{
    event Func<Task>? InitializeRequested;
    event EventHandler? NavigateHomeRequested;
    event EventHandler? OpenDatabaseConfigRequested;
    event EventHandler<IFeatureModule>? ModuleSelected;

    void ShowHomeScreen(IReadOnlyList<IFeatureModule> modules);
    void ShowModuleControl(string moduleTitle, UserControl control);
    void UpdateConnectionStatus(bool isConfigured, string statusText);
}

