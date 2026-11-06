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
2. **Injeção de Dependência Nativa**: Todo acoplamento de serviços, validadores, presenters e telas deve ser registrado via `Microsoft.Extensions.DependencyInjection` em [`ServiceCollectionExtensions.cs`](Infrastructure/DependencyInjection/ServiceCollectionExtensions.cs).
3. **Padrão MVP (Model-View-Presenter)**: A interface gráfica (`UserControl` / `Form`) implementa apenas uma interface de View (ex.: `IMeuCenarioView`) e delega toda orquestração, validação e chamadas de negócio para um `Presenter`.
4. **Componentização com `UserControl`**: Cada cenário funcional deve possuir seu próprio `UserControl` independente e reutilizável. **Nota importante (.NET 10 WinForms):** Propriedades públicas *read/write* em `UserControl` devem ser decoradas com `[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]` para evitar o erro `WFO1000`.
5. **Validação com FluentValidation**: Toda entrada de dados do usuário deve ser representada por um DTO/Model e validada através de uma classe que herda de `AbstractValidator<T>`.
6. **Logs Estruturados com Serilog**: Eventos de execução, auditoria de alterações de integração e erros devem ser registrados via `ILogger<T>` (integrado ao **Serilog**), gravando logs diários em `%LocalAppData%\UneContIntegrationsTools\logs\`.
7. **Responsividade Assíncrona (`async/await`)**: Todas as operações de I/O e banco de dados devem ser assíncronas (`Task`), bloqueando os botões da tela com feedback visual (`SetBusyState`) durante a execução para nunca travar a UI thread.
8. **Documentação Viva**: Sempre mantenha o `README.md` atualizado a cada nova funcionalidade ou alteração arquitetural.

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

## 3. Conceitos Arquiteturais e Recursos do .NET 10 Aplicados

A arquitetura do projeto combina **Vertical Slice Architecture**, **MVP (Model-View-Presenter - Passive View)** e **Injeção de Dependência**, trazendo práticas modernas de engenharia de software para o Windows Forms sem abrir mão da compatibilidade com o Visual Studio Designer.

### 3.1. Por que Vertical Slices + MVP Passive View no Windows Forms?
- **Vertical Slices (`Features/`)**: Em vez de separar o sistema inteiro em camadas horizontais rígidas (`UI`, `BLL`, `DAL`), cada funcionalidade de negócio vive isolada em sua própria subpasta dentro de `Features/` com sua View, seu Presenter, seus DTOs, seu Validator e seu Service. Isso reduz o acoplamento e facilita a manutenção.
- **MVP Passive View**:
  - **View (`UserControl` / `Form`)**: Cuida exclusivamente de componentes visuais (`TextBox`, `ComboBox`, `DataGridView`, `ErrorProvider`), expondo dados por propriedades e ações do usuário por eventos definidos em uma interface (`I...View`). Não contém SQL nem regras de negócio.
  - **Presenter**: Escuta os eventos da View, lê os dados em DTOs, aciona o `FluentValidation`, chama os serviços assíncronos (`Services` / `ISqlConnectionFactory`) e comanda a atualização visual da View.
  - **Model / DTO**: Objetos imutáveis (`record`) que transportam dados entre a tela, validadores e banco de dados.

### 3.2. Casca Principal Estilo SPA Desktop e Módulos Plugáveis (`IFeatureModule`)
- **`MainForm` como Shell (MasterPage)**: Em vez de abrir múltiplas janelas modais (`ShowDialog`) ou janelas MDI, o [`MainForm.cs`](UI/MainForm.cs) mantém cabeçalho, *breadcrumb* e barra de status fixos e troca apenas o `UserControl` ativo no painel central (`SwapActiveControl`), executando `.Dispose()` nos controles anteriores para evitar vazamento de memória e *handles* GDI+.
- **Descoberta Automática de Funcionalidades (Princípio Open/Closed - SOLID)**: Qualquer classe que implemente [`IFeatureModule`](Core/Interfaces/IFeatureModule.cs) e seja registrada no container de DI (`services.AddSingleton<IFeatureModule, MeuModulo>()`) é injetada automaticamente via `IEnumerable<IFeatureModule>` no [`MainPresenter`](UI/MainPresenter.cs) e renderizada como um card clicável no [`HomeDashboardControl`](UI/Controls/HomeDashboardControl.cs), sem necessidade de alterar a tela inicial.

### 3.3. Injeção de Dependência e Ciclo de Vida no WinForms
- Configurada em [`ServiceCollectionExtensions.cs`](Infrastructure/DependencyInjection/ServiceCollectionExtensions.cs) usando `Microsoft.Extensions.DependencyInjection`.
- Serviços globais de estado e infraestrutura (como [`SqlConnectionFactory`](Infrastructure/Database/SqlConnectionFactory.cs) e validadores) são registrados como **`Singleton`**, garantindo que a conexão ativa fique disponível para todos os módulos.
- Telas de funcionalidades (`UserControl`) e serviços de consulta são registrados como **`Transient`**, sendo recriados de forma limpa a cada navegação.
- O método `ActivatorUtilities.CreateInstance<TPresenter>(serviceProvider, viewControl)` é utilizado para instanciar cada `Presenter` combinando a instância específica do `UserControl` recém-criado com as demais dependências resolvidas automaticamente pelo container (`ILogger<T>`, `IValidator<T>`, `ISqlConnectionFactory`).

### 3.4. Recursos Modernos do C# (.NET 10) Utilizados no Código
- **Primary Constructors (C# 12)**: Declaração de dependências diretamente na assinatura da classe (ex.: `public sealed class FilaIntegracaoService(ISqlConnectionFactory connectionFactory, ILogger<FilaIntegracaoService> logger)`).
- **Records Imutáveis**: DTOs concisos e seguros para transporte de filtros e resultados (ex.: [`FilaIntegracaoDtos.cs`](Features/Integracoes/Models/FilaIntegracaoDtos.cs)).
- **Raw String Literals (`""" ... """`)**: Queries SQL multilinhas legíveis e sem necessidade de escape de aspas em [`FilaIntegracaoService.cs`](Features/Integracoes/Services/FilaIntegracaoService.cs).
- **`ReadOnlySpan<T>` e Collection Expressions (`[...]`)**: Iteração de coleções estáticas na *Stack* sem alocação desnecessária no *Heap* (ex.: registro de providers em [`SqlConnectionFactory.cs`](Infrastructure/Database/SqlConnectionFactory.cs)).
- **`Switch Expressions` e Pattern Matching**: Despacho declarativo de modos de autenticação e configuração visual de controles.
- **`await using` (`IAsyncDisposable`)**: Liberação assíncrona de conexões, comandos e leitores (`SqlConnection`, `SqlCommand`, `SqlDataReader`) sem bloquear a thread de UI.
- **`System.Threading.Lock` (.NET 9/10)**: Primitiva moderna e otimizada de sincronização entre threads utilizada em [`SqlConnectionFactory.cs`](Infrastructure/Database/SqlConnectionFactory.cs).

---

## 4. Acesso ao Banco de Dados (SQL Server & Azure / Microsoft Entra ID)

O acesso ao banco de dados utiliza o driver oficial **`Microsoft.Data.SqlClient` (7.x)** em conjunto com o pacote **`Microsoft.Data.SqlClient.Extensions.Azure` (7.x)** e é centralizado no serviço [`ISqlConnectionFactory`](Core/Interfaces/ISqlConnectionFactory.cs).

A construção da conexão em [`SqlConnectionFactory.cs`](Infrastructure/Database/SqlConnectionFactory.cs) mantém **separados** os métodos de comunicação direta ao SQL Server (por usuário e senha) e os métodos de autenticação corporativa via Azure / Microsoft Entra ID (baseados em token, sem tráfego de senha corporativa na aplicação e sem uso do fluxo legado/obsoleto `ActiveDirectoryPassword` / `CS0618`). A seleção do usuário define qual forma de comunicação será utilizada:

1. **Comunicação Direta SQL Server (`BuildSqlServerPasswordConnectionString`)**:
   - **SQL Server (Usuário e Senha)** (`SqlAuthenticationMode.SqlServerPassword`): Autenticação nativa diretamente no SQL Server (`IntegratedSecurity = false`, `UserID` e `Password`).
2. **Comunicação via Azure / Microsoft Entra ID (`BuildAzureEntraConnectionString`)**:
   - **Azure / Microsoft Entra ID - Interativo (MFA / Navegador)** (`SqlAuthenticationMode.MicrosoftEntraInteractive` → `SqlAuthenticationMethod.ActiveDirectoryInteractive`): Abre o prompt seguro da Microsoft com suporte a MFA/Conditional Access e obtém um token OAuth2/OIDC via `ActiveDirectoryAuthenticationProvider`.
   - **Azure / Microsoft Entra ID - Integrado / Default** (`SqlAuthenticationMode.MicrosoftEntraDefault` → `SqlAuthenticationMethod.ActiveDirectoryDefault`): Autentica automaticamente utilizando as credenciais do ambiente (Windows, Visual Studio, Azure CLI ou Managed Identity).

As configurações são salvas localmente em `%LocalAppData%\UneContIntegrationsTools\connection-settings.json`, e quando o modo **SQL Server (Usuário e Senha)** é utilizado com a opção "Lembrar senha", a senha é criptografada com **Windows DPAPI (`ProtectedData.Protect` no escopo `CurrentUser`)**, impedindo que o arquivo seja lido em outra máquina ou por outro usuário do Windows.

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

## 5. Fluxo de Desenvolvimento para Incluir Novas Funcionalidades

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
Registre o Validador, o Serviço, o `UserControl` e o `IFeatureModule` em [`ServiceCollectionExtensions.cs`](Infrastructure/DependencyInjection/ServiceCollectionExtensions.cs):

```csharp
services.AddSingleton<IValidator<ReprocessarNotaDto>, ReprocessarNotaValidator>();
services.AddTransient<IReprocessarNotaService, ReprocessarNotaService>();
services.AddTransient<ReprocessarNotaControl>();
services.AddSingleton<IFeatureModule, ReprocessarNotaModule>();
```

Ao registrar `IFeatureModule`, o botão do novo cenário aparecerá **automaticamente** na tela inicial (`HomeDashboardControl`) sem necessidade de alterar `MainForm` ou `MainPresenter`.

---

## 6. Funcionalidades Implementadas

### 6.1. Módulo `Integrações` (`Features/Integracoes/`)
- **Botão na Tela Inicial**: `Integrações` ([`IntegracoesModule`](Features/Integracoes/IntegracoesModule.cs)).
- **Subtela `Integrações` ([`IntegracoesHubControl`](Features/Integracoes/IntegracoesHubControl.cs))**: Central que agrupa os botões de ferramentas de integração.
  - **Botão `Fila de integração` ([`FilaIntegracaoControl`](Features/Integracoes/FilaIntegracaoControl.cs))**:
    - Permite alternar o tipo de nota consultada entre **Serviços Tomados** (`NFe.ServicoTomado`) e **Serviços Prestados** (`NFe.ServicoPrestado`).
    - Consulta e exibe a listagem agrupada de sistemas de integração (`INTEGRACAO`) e quantidade de notas na fila (`QTDE_NOTAS`) ordenadas de forma decrescente.
    - Permite filtrar por `Nº Tentativa Mínimo` (`FILA.NumeroTentativa >= @NumeroTentativaMinimo`, padrão `0`) e busca rápida por nome da integração.
    - Exibe KPIs no topo (Total de Sistemas na Fila, Total de Notas e Horário/Tempo da Consulta).

---

## 7. Como Compilar e Executar

Na raiz do repositório, execute:

```powershell
dotnet build UneContIntegrationsTools.slnx
dotnet run --project UneContIntegrationsTools.csproj
```

