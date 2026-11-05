using Microsoft.Extensions.DependencyInjection;
using Serilog;
using UneContIntegrationsTools.Infrastructure.DependencyInjection;
using UneContIntegrationsTools.UI;

namespace UneContIntegrationsTools;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        var logFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "UneContIntegrationsTools",
            "logs",
            "unecont-tools-.log");

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .Enrich.FromLogContext()
            .WriteTo.File(
                path: logFilePath,
                rollingInterval: RollingInterval.Day,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}")
            .CreateLogger();

        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
        Application.ThreadException += (_, e) =>
        {
            Log.Error(e.Exception, "Exceção não tratada na thread de interface gráfica.");
            MessageBox.Show(
                $"Ocorreu um erro inesperado:\n{e.Exception.Message}",
                "UneCont Integration Tools - Erro",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        };

        AppDomain.CurrentDomain.UnhandledException += (_, e) =>
        {
            if (e.ExceptionObject is Exception ex)
            {
                Log.Fatal(ex, "Exceção fatal não tratada no domínio da aplicação.");
            }
        };

        try
        {
            Log.Information("Iniciando aplicação UneContIntegrationTools.");
            ApplicationConfiguration.Initialize();

            var services = new ServiceCollection();
            services.AddApplicationServices();

            using var serviceProvider = services.BuildServiceProvider();

            var mainForm = serviceProvider.GetRequiredService<MainForm>();
            _ = serviceProvider.GetRequiredService<MainPresenter>();

            Application.Run(mainForm);
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Falha crítica na inicialização da aplicação.");
            throw;
        }
        finally
        {
            Log.Information("Encerrando aplicação UneContIntegrationTools.");
            Log.CloseAndFlush();
        }
    }
}
