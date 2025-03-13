namespace Examples.MarkdownLinkScanWorker.Services;

internal sealed class MarkdigScanService(
    ILogger<MarkdigScanService> logger,
    Configuration configuration
) : IScanService
{
    public async IAsyncEnumerable<string> GetHyperlinksAsync()
    {
        logger.LogMarkdigScanStarted(configuration.Path);

        await Task.CompletedTask;

        foreach (var link in new string[] { "https://github.com/xoofx/markdig", "https://www.nuget.org/packages/markdig/" })
        {
            yield return link;
        }
    }
}

internal static partial class Logging
{
    private static readonly Action<ILogger, string, Exception?> LogMarkdigScanStartedAction =
        LoggerMessage.Define<string>(
            LogLevel.Information,
            new EventId(default, nameof(MarkdigScanService)),
            "Scan started using Markdig for path: {Path}");

    public static void LogMarkdigScanStarted(this ILogger logger, string path)
    {
        if (logger.IsEnabled(LogLevel.Information))
        {
            LogMarkdigScanStartedAction(logger, path, default);
        }
    }
}