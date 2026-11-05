# UneCont Integration Tools

Ferramenta desktop em **Windows Forms (.NET 10 / C#)** desenvolvida para auxiliar a equipe de integração e engenharia na resolução rápida, segura e padronizada de tickets. 

O sistema é **estritamente modular**:
- **Barra Superior (`MainForm`)**: Contém a navegação para a tela inicial (`⊞ Tela Inicial (Funcionalidades)`) e o botão dedicado de configuração de banco de dados (`⚙ Conexão SQL / Entra ID`), mantendo a configuração de infraestrutura fora da grade de cenários.
- **Painel Central (`HomeDashboardControl`)**: Exclusivo para os botões/cards de cada cenário de integração implementado via `IFeatureModule`.
- **Barra de Status Inferior**: Exibe em tempo real o servidor, banco e modo de autenticação SQL Server / Microsoft Entra ID ativo.

---

## 1. Diretrizes Técnicas Obrigatórias

Toda implementação ou manutenção neste repositório deve respeitar as seguintes diretrizes:

1. **C# e .NET 10**: Utilize recursos modernos da linguagem (Primary Constructors, Records, Pattern Matching, Collection Expressions, Nullable Reference Types habilitados).
2. **Injeção de Dependência Nativa**: Todo acoplamento de serviços, validadores, presenters e telas deve ser registrado via `Microsoft.Extensions.DependencyInjection` em [`ServiceCollectionExtensions.cs`](UneContIntegrationsTools/Infrastructure/DependencyInjection/ServiceCollectionExtensions.cs).
3. **Padrão MVP (Model-View-Presenter)**: A interface gráfica (`UserControl` / `Form`) implementa apenas uma interface de View (ex.: `IMeuCenarioView`) e delega toda orquestração, validação e chamadas de negócio para um `Presenter`.
4. **Componentização com `UserControl`**: Cada cenário funcional deve possuir seu próprio `UserControl` independente e reutilizável. **Nota importante (.NET 10 WinForms):** Propriedades públicas *read/write* em `UserControl` devem ser decoradas com `[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]` para evitar o erro `WFO1000`.
5. **Validação com FluentValidation**: Toda entrada de dados do usuário deve ser representada por um DTO/Model e validada através de uma classe que herda de `AbstractValidator<T>`.
6. **Logs Estruturados com Serilog**: Eventos de execução, auditoria de alterações de integração e erros devem ser registrados via `ILogger<T>` (integrado ao **Serilog**), gravando logs diários em `%LocalAppData%\UneContIntegrationsTools\logs\`.
7. **Responsividade Assíncrona (`async/await`)**: Todas as operações de I/O e banco de dados devem ser assíncronas (`Task`), bloqueando os botões da tela com feedback visual (`SetBusyState`) durante a execução para nunca travar a UI thread.
8. **Documentação Viva**: Sempre mantenha o `README.md` e os arquivos em `.contexto/` atualizados a cada nova funcionalidade ou alteração arquitetural.

---

## 2. Estrutura do Projeto

```text
UneContIntegrationsTools/
├── Core/
│   ├── Interfaces/                 # Contratos globais (IFeatureModule, ISqlConnectionFactory, etc.)
│   ├── Models/                     # DTOs e Enums compartilhados (SqlConnectionSettingsDto, etc.)
│   └── Validators/                 # Validadores FluentValidation globais
├── Infrastructure/
│   ├── Database/                   # Implementação de acesso ao SQL Server / Microsoft Entra ID
│   └── DependencyInjection/        # Registro central de DI (ServiceCollectionExtensions.cs)
├── Features/                       # Módulos funcionais (cada cenário fica em sua subpasta)
│   ├── DatabaseConfig/             # Módulo de configuração e diagnóstico de conexão SQL / Entra ID (Botão superior)
│   └── Integracoes/                # Módulo de Integrações (Hub + Fila de Integração)
├── UI/
│   ├── Controls/                   # Componentes da casca principal (HomeDashboardControl)
│   ├── IMainView.cs                # Contrato MVP da janela principal
│   ├── MainPresenter.cs            # Presenter de navegação e status global
│   └── MainForm.cs                 # Janela principal da aplicação
└── Program.cs                      # Ponto de entrada, configuração do Serilog e container DI
```

---

## 3. Acesso ao Banco de Dados (SQL Server & Microsoft Entra ID)

O acesso ao banco de dados é centralizado no serviço [`ISqlConnectionFactory`](UneContIntegrationsTools/Core/Interfaces/ISqlConnectionFactory.cs), que suporta:

- **SQL Server (Usuário e Senha)** (`SqlAuthenticationMode.SqlServerPassword`)
- **Microsoft Entra ID - Interativo / MFA** (`SqlAuthenticationMode.MicrosoftEntraInteractive`)
- **Microsoft Entra ID - Usuário e Senha** (`SqlAuthenticationMode.MicrosoftEntraPassword`)
- **Microsoft Entra ID - Integrado / Default** (`SqlAuthenticationMode.MicrosoftEntraDefault`)

As configurações são salvas localmente em `%LocalAppData%\UneContIntegrationsTools\connection-settings.json`, com a senha criptografada via **Windows DPAPI (`ProtectedData`)**.

### Como utilizar a conexão SQL em qualquer novo cenário:

Basta injetar `ISqlConnectionFactory` no seu serviço de negócio e chamar `CreateOpenConnectionAsync`:

```csharp
public sealed class MeuCenarioService(
    ISqlConnectionFactory connectionFactory,
    ILogger<MeuCenarioService> logger) : IMeuCenarioService
{
    public async Task ExecutarCorrecaoAsync(MeuCenarioRequestDto request, CancellationToken ct = default)
    {
        await using var connection = await connectionFactory.CreateOpenConnectionAsync(ct);
        await using var command = connection.CreateCommand();
        command.CommandText = "UPDATE Tabela SET Status = @Status WHERE Id = @Id";
        command.Parameters.AddWithValue("@Status", request.NovoStatus);
        command.Parameters.AddWithValue("@Id", request.RegistroId);

        var rows = await command.ExecuteNonQueryAsync(ct);
        logger.LogInformation("Ticket corrigido para Id {RegistroId}. Linhas afetadas: {Rows}", request.RegistroId, rows);
    }
}
```

---

## 4. Fluxo de Desenvolvimento para Incluir Novas Funcionalidades

Sempre que um novo cenário para resolução de tickets for solicitado, siga rigorosamente estas **5 etapas**:

### Passo 1: Esclarecimento de Regras
Verifique se todas as entradas, tabelas envolvidas, validações e critérios de sucesso estão claros antes de codificar.

### Passo 2: Definir Entidades, Contratos, DTOs e Validador
Crie uma nova pasta em `Features/<NomeDoCenario>/` contendo:
- **DTOs de Entrada/Saída**: `record`s imutáveis representando os dados do cenário.
- **Validador (`FluentValidation`)**: Classe herdando de `AbstractValidator<TRequestDto>`.
- **Contrato de Serviço (`IMeuCenarioService`)** e **Contrato da View (`IMeuCenarioView`)**.

```csharp
public sealed record ReprocessarNotaDto(string Cnpj, int NumeroNota);

public sealed class ReprocessarNotaValidator : AbstractValidator<ReprocessarNotaDto>
{
    public ReprocessarNotaValidator()
    {
        RuleFor(x => x.Cnpj).NotEmpty().Length(14);
        RuleFor(x => x.NumeroNota).GreaterThan(0);
    }
}
```

### Passo 3: Projetar o `UserControl` (View) e o `Presenter` (MVP)
- Crie `<NomeDoCenario>Control.cs` e `<NomeDoCenario>Control.Designer.cs` implementando `IMeuCenarioView`.
- Lembre-se de adicionar `[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]` nas propriedades públicas da View.
- Crie `<NomeDoCenario>Presenter.cs` que escuta os eventos da View, valida o DTO via `IValidator<T>`, aciona o serviço de forma assíncrona (`async/await`) e atualiza a View.

### Passo 4: Implementar o Serviço e a Classe `IFeatureModule`
Implemente `IFeatureModule` para expor o módulo no painel de botões da tela inicial:

```csharp
public sealed class ReprocessarNotaModule(IServiceProvider serviceProvider) : IFeatureModule
{
    public string Id => "integração.reprocessar-nota";
    public string Title => "Reprocessar Nota Fiscal";
    public string Description => "Destrava e reenvia notas fiscais pendentes de integração.";
    public string Category => "Integração Fiscal";
    public int DisplayOrder => 10;

    public UserControl CreateControl()
    {
        var control = serviceProvider.GetRequiredService<ReprocessarNotaControl>();
        _ = ActivatorUtilities.CreateInstance<ReprocessarNotaPresenter>(serviceProvider, control);
        return control;
    }
}
```

### Passo 5: Registrar no Container de Injeção de Dependência
Registre o Validador, o Serviço, o `UserControl` e o `IFeatureModule` em [`ServiceCollectionExtensions.cs`](UneContIntegrationsTools/Infrastructure/DependencyInjection/ServiceCollectionExtensions.cs):

```csharp
services.AddSingleton<IValidator<ReprocessarNotaDto>, ReprocessarNotaValidator>();
services.AddTransient<IReprocessarNotaService, ReprocessarNotaService>();
services.AddTransient<ReprocessarNotaControl>();
services.AddSingleton<IFeatureModule, ReprocessarNotaModule>();
```

Ao registrar `IFeatureModule`, o botão do novo cenário aparecerá **automaticamente** na tela inicial (`HomeDashboardControl`) sem necessidade de alterar `MainForm` ou `MainPresenter`.

---

## 5. Funcionalidades Implementadas

### 5.1. Módulo `Integrações` (`Features/Integracoes/`)
- **Botão na Tela Inicial**: `Integrações` ([`IntegracoesModule`](UneContIntegrationsTools/Features/Integracoes/IntegracoesModule.cs)).
- **Subtela `Integrações` ([`IntegracoesHubControl`](UneContIntegrationsTools/Features/Integracoes/IntegracoesHubControl.cs))**: Central que agrupa os botões de ferramentas de integração.
  - **Botão `Fila de integração` ([`FilaIntegracaoControl`](UneContIntegrationsTools/Features/Integracoes/FilaIntegracaoControl.cs))**:
    - Consulta e exibe a listagem agrupada de sistemas de integração (`INTEGRACAO`) e quantidade de notas na fila (`QTDE_NOTAS`) ordenadas de forma decrescente.
    - Permite filtrar por `Nº Tentativa Mínimo` (`FILA.NumeroTentativa >= @NumeroTentativaMinimo`, padrão `0`) e busca rápida por nome da integração.
    - Exibe KPIs no topo (Total de Sistemas na Fila, Total de Notas e Horário/Tempo da Consulta).

---

## 6. Como Compilar e Executar

Na raiz do repositório, execute:

```powershell
dotnet build UneContIntegrationsTools.slnx
dotnet run --project UneContIntegrationsTools/UneContIntegrationsTools.csproj
```

