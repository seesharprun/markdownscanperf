using System.Diagnostics;

namespace Examples.MarkdownLinkScanWorker.Services;

internal sealed class ApplicationWorkerService(
    ILogger<ApplicationWorkerService> logger,
    IScanService searchService,
    IResultsService resultsService,
    IHostApplicationLifetime applicationLifetime
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();

        logger.LogWorkerRunning();

        IAsyncEnumerable<string> hyperlinks = searchService.GetHyperlinksAsync();

        stopwatch.Stop();

        await resultsService.SaveResultsAsync(hyperlinks, stopwatch.Elapsed);

        applicationLifetime.StopApplication();
    }
}

internal static partial class Logging
{
    private static readonly Action<ILogger, Exception?> LogWorkerRunningAction =
        LoggerMessage.Define(
            LogLevel.Information,
            new EventId(default, nameof(ApplicationWorkerService)),
            "Worker started");

    public static void LogWorkerRunning(this ILogger logger)
    {
        if (logger.IsEnabled(LogLevel.Information))
        {
            LogWorkerRunningAction(logger, default);
        }
    }
}