# Prompt para Harness Assistente — Validação e Correção de Autenticação Entra ID no Microsoft.Data.SqlClient (.NET 10)

## Papel do assistente

Você é um assistente técnico especializado em:

- .NET 10
- C#
- Microsoft.Data.SqlClient
- Microsoft Entra ID
- Azure SQL / SQL Server
- Migração de APIs obsoletas
- Validação de warnings `CS0618`
- Pesquisa em documentação oficial, release notes e código-fonte

Sua missão é **validar informações, pesquisar fontes confiáveis, testar quando possível e propor a correção definitiva** para um código que registra providers de autenticação do Microsoft.Data.SqlClient.

---

## Objetivo

Validar se a solução abaixo está correta para um projeto rodando em **.NET 10** e descobrir a forma correta de remover o uso de APIs obsoletas, especialmente os membros antigos:

```csharp
SqlAuthenticationMethod.ActiveDirectoryDefault
SqlAuthenticationMethod.ActiveDirectoryInteractive
SqlAuthenticationMethod.ActiveDirectoryPassword
```

O objetivo final é produzir um código moderno, sem `#pragma warning disable CS0618`, compatível com a versão recomendada do pacote `Microsoft.Data.SqlClient`.

---

## Código original a ser validado

```csharp
private static void EnsureEntraAuthenticationProvidersRegistered()
{
#pragma warning disable CS0618
    ReadOnlySpan<SqlAuthenticationMethod> entraMethods =
    [
        SqlAuthenticationMethod.ActiveDirectoryDefault,
        SqlAuthenticationMethod.ActiveDirectoryInteractive,
        SqlAuthenticationMethod.ActiveDirectoryPassword
    ];
#pragma warning restore CS0618

    ActiveDirectoryAuthenticationProvider? provider = null;

    foreach (var method in entraMethods)
    {
        if (SqlAuthenticationProvider.GetProvider(method) is null)
        {
            provider ??= new ActiveDirectoryAuthenticationProvider();
            SqlAuthenticationProvider.SetProvider(method, provider);
        }
    }
}
```

---

## Hipóteses que precisam ser validadas

A hipótese inicial é que os membros antigos tenham sido substituídos por:

```csharp
SqlAuthenticationMethod.EntraIdDefault
SqlAuthenticationMethod.EntraIdInteractive
SqlAuthenticationMethod.EntraIdPassword
```

Também é necessário validar se o provider correto é:

```csharp
ActiveDirectoryAuthenticationProvider
```

ou se existe um provider mais novo, como:

```csharp
EntraIdAuthenticationProvider
```

Além disso, é necessário validar se ainda é preciso registrar manualmente o provider usando:

```csharp
SqlAuthenticationProvider.SetProvider(method, provider);
```

ou se as versões atuais do `Microsoft.Data.SqlClient` já fazem isso automaticamente.

---

## Perguntas obrigatórias que você deve responder

1. Os membros abaixo existem nas versões atuais do `Microsoft.Data.SqlClient`?

   ```csharp
   SqlAuthenticationMethod.EntraIdDefault
   SqlAuthenticationMethod.EntraIdInteractive
   SqlAuthenticationMethod.EntraIdPassword
   ```

2. Os membros abaixo estão obsoletos?

   ```csharp
   SqlAuthenticationMethod.ActiveDirectoryDefault
   SqlAuthenticationMethod.ActiveDirectoryInteractive
   SqlAuthenticationMethod.ActiveDirectoryPassword
   ```

3. Qual é a mensagem oficial de obsolescência, se existir?

4. Qual é a substituição oficial recomendada pela Microsoft?

5. Qual versão do `Microsoft.Data.SqlClient` deve ser usada com .NET 10?

   Exemplos:

   - 6.x?
   - 7.x?
   - Versão mais recente?

6. O tipo abaixo ainda existe?

   ```csharp
   ActiveDirectoryAuthenticationProvider
   ```

7. O tipo abaixo está obsoleto?

   ```csharp
   ActiveDirectoryAuthenticationProvider
   ```

8. Existe um tipo chamado:

   ```csharp
   EntraIdAuthenticationProvider
   ```

   no `Microsoft.Data.SqlClient`?

9. Se `EntraIdAuthenticationProvider` existir, ele deve ser usado no lugar de:

   ```csharp
   ActiveDirectoryAuthenticationProvider
   ```

10. O registro manual abaixo ainda é necessário?

    ```csharp
    SqlAuthenticationProvider.SetProvider(method, provider);
    ```

11. Se o registro manual não for necessário, qual é o comportamento padrão do `Microsoft.Data.SqlClient`?

12. Os valores antigos `ActiveDirectory*` e os novos `EntraId*` são aliases com o mesmo valor numérico?

13. Registrar os providers usando os novos valores `EntraId*` também cobre cenários que ainda usem connection strings antigas com `Active Directory Default`, `Active Directory Interactive` ou `Active Directory Password`?

14. Qual é o código final recomendado para .NET 10?

15. Existe algum cenário em que ainda seja necessário manter `#pragma warning disable CS0618` temporariamente?

---

## Fontes que devem ser priorizadas

Pesquise e valide as informações preferencialmente nestas fontes:

1. Microsoft Learn

   Exemplo:

   ```text
   https://learn.microsoft.com/
   ```

2. Documentação oficial do `Microsoft.Data.SqlClient`

3. NuGet do pacote `Microsoft.Data.SqlClient`

   ```text
   https://www.nuget.org/packages/Microsoft.Data.SqlClient
   ```

4. Repositório oficial do Microsoft.Data.SqlClient no GitHub

   ```text
   https://github.com/dotnet/SqlClient
   ```

5. Release notes oficiais do pacote

6. Código-fonte do `Microsoft.Data.SqlClient`

7. Blogs oficiais da Microsoft, quando necessário

Evite respostas baseadas apenas em memória. Sempre que possível, cite links, arquivos, classes, enums ou trechos de release notes.

---

## Validação prática recomendada

Se o ambiente permitir execução de comandos, crie um projeto mínimo para validar a compilação.

### Passos sugeridos

```bash
dotnet new console -n SqlClientEntraValidation -f net10.0
cd SqlClientEntraValidation
dotnet add package Microsoft.Data.SqlClient
```

Depois, adicione um arquivo de teste contendo código semelhante a:

```csharp
using Microsoft.Data.SqlClient;

static class Program
{
    static void Main()
    {
        EnsureEntraAuthenticationProvidersRegistered();
    }

    private static void EnsureEntraAuthenticationProvidersRegistered()
    {
        ReadOnlySpan<SqlAuthenticationMethod> entraMethods =
        [
            SqlAuthenticationMethod.EntraIdDefault,
            SqlAuthenticationMethod.EntraIdInteractive,
            SqlAuthenticationMethod.EntraIdPassword
        ];

        SqlAuthenticationProvider? provider = null;

        foreach (SqlAuthenticationMethod method in entraMethods)
        {
            if (SqlAuthenticationProvider.GetProvider(method) is null)
            {
                provider ??= new ActiveDirectoryAuthenticationProvider();
                SqlAuthenticationProvider.SetProvider(method, provider);
            }
        }
    }
}
```

Compile com:

```bash
dotnet build
```

Se o build falhar, ajuste o código testando alternativas como:

```csharp
new EntraIdAuthenticationProvider()
```

ou:

```csharp
new ActiveDirectoryAuthenticationProvider()
```

conforme a versão instalada do pacote.

---

## Critérios de aceite

A resposta final só deve ser considerada válida se:

- [ ] Confirmar se `EntraIdDefault` existe.
- [ ] Confirmar se `EntraIdInteractive` existe.
- [ ] Confirmar se `EntraIdPassword` existe.
- [ ] Confirmar se `ActiveDirectoryDefault` está obsoleto.
- [ ] Confirmar se `ActiveDirectoryInteractive` está obsoleto.
- [ ] Confirmar se `ActiveDirectoryPassword` está obsoleto.
- [ ] Confirmar se `ActiveDirectoryAuthenticationProvider` existe.
- [ ] Confirmar se `ActiveDirectoryAuthenticationProvider` está obsoleto.
- [ ] Confirmar se `EntraIdAuthenticationProvider` existe.
- [ ] Informar qual versão do `Microsoft.Data.SqlClient` foi validada.
- [ ] Apresentar o código final recomendado.
- [ ] Apresentar código alternativo, se houver diferença entre versões.
- [ ] Explicar se o método `EnsureEntraAuthenticationProvidersRegistered` ainda é necessário.
- [ ] Apresentar fontes oficiais ou confiáveis.

---

## Formato obrigatório da resposta

Responda exatamente neste formato:

```text
## Resumo da validação

[Explique se a solução anterior está correta, parcialmente correta ou incorreta.]

## Versão do Microsoft.Data.SqlClient validada

[Informe a versão do pacote usada ou recomendada.]

## APIs confirmadas

[Liste quais tipos e membros existem, com evidência.]

## APIs obsoletas

[Liste quais APIs estão obsoletas e qual é a substituição oficial.]

## Provider correto

[Informe se deve usar ActiveDirectoryAuthenticationProvider, EntraIdAuthenticationProvider ou outro.]

## Registro manual ainda é necessário?

[Sim/Não, com explicação.]

## Código recomendado

[Apresente o código final corrigido.]

## Código alternativo para versões antigas

[Se necessário, apresente fallback.]

## Fontes

[Liste links ou referências usadas.]

## Parecer final

[Conclusão objetiva.]
```

---

## Código recomendado esperado

Se a API nova `EntraId*` existir e o provider continuar sendo `ActiveDirectoryAuthenticationProvider`, o código esperado será semelhante a este:

```csharp
using Microsoft.Data.SqlClient;

private static void EnsureEntraAuthenticationProvidersRegistered()
{
    ReadOnlySpan<SqlAuthenticationMethod> entraMethods =
    [
        SqlAuthenticationMethod.EntraIdDefault,
        SqlAuthenticationMethod.EntraIdInteractive,
        SqlAuthenticationMethod.EntraIdPassword
    ];

    SqlAuthenticationProvider? provider = null;

    foreach (SqlAuthenticationMethod method in entraMethods)
    {
        if (SqlAuthenticationProvider.GetProvider(method) is null)
        {
            provider ??= new ActiveDirectoryAuthenticationProvider();
            SqlAuthenticationProvider.SetProvider(method, provider);
        }
    }
}
```

Se existir um provider mais novo chamado `EntraIdAuthenticationProvider`, o código esperado poderá ser:

```csharp
using Microsoft.Data.SqlClient;

private static void EnsureEntraAuthenticationProvidersRegistered()
{
    ReadOnlySpan<SqlAuthenticationMethod> entraMethods =
    [
        SqlAuthenticationMethod.EntraIdDefault,
        SqlAuthenticationMethod.EntraIdInteractive,
        SqlAuthenticationMethod.EntraIdPassword
    ];

    SqlAuthenticationProvider? provider = null;

    foreach (SqlAuthenticationMethod method in entraMethods)
    {
        if (SqlAuthenticationProvider.GetProvider(method) is null)
        {
            provider ??= new EntraIdAuthenticationProvider();
            SqlAuthenticationProvider.SetProvider(method, provider);
        }
    }
}
```

Se o registro manual não for mais necessário, a resposta deve informar se o método pode ser removido completamente.

---

## Regras importantes

1. Não invente nomes de tipos, métodos ou enums.
2. Se não conseguir confirmar uma informação, diga explicitamente que não conseguiu confirmar.
3. Se existir diferença entre versões do pacote, explique claramente.
4. Se a documentação estiver contraditória, mostre os dois lados e indique a fonte mais confiável.
5. Priorize sempre a solução que compile sem warnings em .NET 10.
6. Não recomende suprimir `CS0618` como solução definitiva, exceto se houver justificativa temporária.
7. Se a solução correta for remover o método, explique por quê.
8. Se a solução correta for apenas trocar os enums, explique por quê.
9. Se a solução correta for trocar o provider, explique por quê.
10. Sempre apresente o código final pronto para uso.

---

## Checklist final

Antes de responder, confirme:

```text
[ ] O projeto usa .NET 10.
[ ] A versão do Microsoft.Data.SqlClient foi identificada.
[ ] SqlAuthenticationMethod.EntraIdDefault foi validado.
[ ] SqlAuthenticationMethod.EntraIdInteractive foi validado.
[ ] SqlAuthenticationMethod.EntraIdPassword foi validado.
[ ] ActiveDirectoryAuthenticationProvider foi validado.
[ ] EntraIdAuthenticationProvider foi verificado.
[ ] A necessidade de SetProvider foi verificada.
[ ] O código final compila sem CS0618.
[ ] As fontes foram citadas.
```

---

## Instrução final

Pesquise, valide, teste se possível e apresente a correção definitiva para o código acima, compatível com .NET 10 e com a versão recomendada do `Microsoft.Data.SqlClient`.