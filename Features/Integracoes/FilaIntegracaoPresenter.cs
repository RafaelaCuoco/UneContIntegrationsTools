using FluentValidation;
using Microsoft.Extensions.Logging;
using UneContIntegrationsTools.Core.Interfaces;
using UneContIntegrationsTools.Features.Integracoes.Models;
using UneContIntegrationsTools.Features.Integracoes.Services;

namespace UneContIntegrationsTools.Features.Integracoes;

/// <summary>
/// Presenter (MVP) responsável por orquestrar a validação e consulta assíncrona da Fila de Integração.
/// </summary>
public sealed class FilaIntegracaoPresenter
{
    private readonly IFilaIntegracaoView _view;
    private readonly IFilaIntegracaoService _service;
    private readonly ISqlConnectionFactory _connectionFactory;
    private readonly IValidator<FilaIntegracaoFiltroDto> _validator;
    private readonly ILogger<FilaIntegracaoPresenter> _logger;

    public FilaIntegracaoPresenter(
        IFilaIntegracaoView view,
        IFilaIntegracaoService service,
        ISqlConnectionFactory connectionFactory,
        IValidator<FilaIntegracaoFiltroDto> validator,
        ILogger<FilaIntegracaoPresenter> logger)
    {
        _view = view;
        _service = service;
        _connectionFactory = connectionFactory;
        _validator = validator;
        _logger = logger;

        _view.LoadRequested += OnQueryRequestedAsync;
        _view.RefreshRequested += OnQueryRequestedAsync;
    }

    private async Task OnQueryRequestedAsync()
    {
        if (!_connectionFactory.IsConfigured)
        {
            _view.ShowError("Nenhuma conexão SQL Server ativa foi encontrada. Clique em '⚙ Conexão SQL / Entra ID' no topo da janela para configurar o acesso ao banco.");
            return;
        }

        var filtro = new FilaIntegracaoFiltroDto(
            NumeroTentativaMinimo: _view.NumeroTentativaMinimo,
            BuscaIntegracao: _view.BuscaIntegracao);

        _view.ClearValidationErrors();
        var validationResult = await _validator.ValidateAsync(filtro);
        if (!validationResult.IsValid)
        {
            var msg = string.Join(Environment.NewLine, validationResult.Errors.Select(e => e.ErrorMessage));
            _view.ShowValidationErrors(msg);
            return;
        }

        try
        {
            _view.SetBusyState(true, "Consultando fila de integração no SQL Server...");
            var resumo = await _service.ConsultarFilaAsync(filtro);
            _view.DisplayResult(resumo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao consultar a Fila de Integração.");
            _view.ShowError($"Erro ao consultar a fila de integração:{Environment.NewLine}{ex.Message}");
        }
        finally
        {
            _view.SetBusyState(false);
        }
    }
}

