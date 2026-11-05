namespace UneContIntegrationsTools.Core.Interfaces;

/// <summary>
/// Contrato para módulos de funcionalidades (cenários de suporte) integrados à tela inicial.
/// </summary>
public interface IFeatureModule
{
    /// <summary>
    /// Identificador único do módulo.
    /// </summary>
    string Id { get; }

    /// <summary>
    /// Título exibido no botão da tela inicial.
    /// </summary>
    string Title { get; }

    /// <summary>
    /// Descrição curta exibida abaixo do título no botão/card da tela inicial.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Categoria ou grupo funcional do módulo.
    /// </summary>
    string Category { get; }

    /// <summary>
    /// Ordem de exibição na grade da tela inicial.
    /// </summary>
    int DisplayOrder { get; }

    /// <summary>
    /// Cria e retorna a instância do UserControl associado ao cenário.
    /// </summary>
    UserControl CreateControl();
}

