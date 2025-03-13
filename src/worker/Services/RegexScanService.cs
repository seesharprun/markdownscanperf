namespace Examples.MarkdownLinkScanWorker.Services;

internal sealed class RegexScanService(
    ILogger<RegexScanService> logger,
    Configuration configuration
) : IScanService
{
    public async IAsyncEnumerable<string> GetHyperlinksAsync()
    {
        logger.LogMarkdigScanStarted(configuration.Path);

        await Task.CompletedTask;

        foreach (var link in new string[] { "https://learn.microsoft.com/dotnet/standard/base-types/regular-expressions" })
        {
            yield return link;
        }
    }
}

internal static partial class Logging
{
    private static readonly Action<ILogger, string, Exception?> LogRegexScanStartedAction =
        LoggerMessage.Define<string>(
            LogLevel.Information,
            new EventId(default, nameof(RegexScanService)),
            "Scan started using Regex for path: {Path}");

    public static void LogRegexScanStarted(this ILogger logger, string path)
    {
        if (logger.IsEnabled(LogLevel.Information))
        {
            LogRegexScanStartedAction(logger, path, default);
        }
    }
}