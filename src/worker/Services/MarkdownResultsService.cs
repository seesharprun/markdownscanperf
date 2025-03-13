namespace Examples.MarkdownLinkScanWorker.Services;

internal sealed class MarkdownResultsService(
    ILogger<MarkdownResultsService> logger,
    ICoreService coreService,
    Configuration configuration
) : IResultsService
{
    public async Task SaveResultsAsync(IList<string> hyperlinks, TimeSpan elapsedTime)
    {
        logger.LogStopwatchInformation(elapsedTime);

        List<string> uniqueHyperlinks = [.. hyperlinks.Distinct().Order()];

        coreService.Summary.AddNewLine();
        coreService.Summary.AddMarkdownHeading($"{configuration.Tool}", level: 3);
        if (configuration.OutputHyperlinks)
        {
            coreService.Summary.AddNewLine();
            coreService.Summary.AddRawMarkdown($"The following unique absolute hyperlinks were found in the markdown files:");
            coreService.Summary.AddNewLine();
            coreService.Summary.AddNewLine();
            coreService.Summary.AddMarkdownList(uniqueHyperlinks, ordered: true);
        }
        coreService.Summary.AddNewLine();
        coreService.Summary.AddMarkdownQuote($"Total hyperlinks: {hyperlinks.Count}");
        coreService.Summary.AddMarkdownQuote($"Unique hyperlinks: {uniqueHyperlinks.Count}");
        coreService.Summary.AddMarkdownQuote($"Elapsed time: {elapsedTime.Humanize(3)}");

        if (Summary.IsAvailable)
        {
            await coreService.Summary.WriteAsync();
        }
        logger.LogMarkdownSummary(coreService.Summary.Stringify());
    }
}

internal static partial class Logging
{
    private static readonly Action<ILogger, TimeSpan, Exception?> LogStopwatchInformationAction =
        LoggerMessage.Define<TimeSpan>(
            LogLevel.Information,
            new EventId(default, nameof(MarkdownResultsService)),
            "Elapsed time: {Elapsed:c}");

    private static readonly Action<ILogger, string, Exception?> LogMarkdownSummaryAction =
        LoggerMessage.Define<string>(
            LogLevel.Information,
            new EventId(default, nameof(MarkdownResultsService)),
            "{Summary}");

    public static void LogMarkdownSummary(this ILogger logger, string markdown)
    {
        if (logger.IsEnabled(LogLevel.Information))
        {
            LogMarkdownSummaryAction(logger, markdown, default);
        }
    }

    public static void LogStopwatchInformation(this ILogger logger, TimeSpan elapsed)
    {
        if (logger.IsEnabled(LogLevel.Information))
        {
            LogStopwatchInformationAction(logger, elapsed, default);
        }
    }
}