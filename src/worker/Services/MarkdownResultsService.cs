using System.Text;
using Actions.Core.Services;

namespace Examples.MarkdownLinkScanWorker.Services;

internal sealed class MarkdownResultsService(
    ILogger<MarkdownResultsService> logger,
    ICoreService coreService,
    Configuration configuration
) : IResultsService
{
    public async Task SaveResultsAsync(IAsyncEnumerable<string> results, TimeSpan elapsedTime)
    {
        List<string> hyperlinks = [];

        await foreach (var result in results)
        {
            hyperlinks.Add(result);
        }

        logger.LogStopwatchInformation(elapsedTime);

        logger.LogHyperlinks($"{string.Join(Environment.NewLine, $"- {hyperlinks}")}");

        coreService.Summary.AddMarkdownHeading($"{configuration.Tool}", level: 3);
        coreService.Summary.AddNewLine();
        coreService.Summary.AddRawMarkdown($"The following hyperlinks were found in the markdown files:");
        coreService.Summary.AddNewLine();
        coreService.Summary.AddMarkdownList(hyperlinks, ordered: true);
        coreService.Summary.AddNewLine();
        coreService.Summary.AddMarkdownQuote($"Elapsed time: {elapsedTime:c}");

        await coreService.Summary.WriteAsync();
    }
}

internal static partial class Logging
{
    private static readonly Action<ILogger, string, Exception?> LogHyperlinksAction =
        LoggerMessage.Define<string>(
            LogLevel.Information,
            new EventId(default, nameof(MarkdownResultsService)),
            "---Hyperlinks found---{Hyperlinks}");

    private static readonly Action<ILogger, TimeSpan, Exception?> LogStopwatchInformationAction =
        LoggerMessage.Define<TimeSpan>(
            LogLevel.Information,
            new EventId(default, nameof(MarkdownResultsService)),
            "Elapsed time: {Elapsed:c}");

    public static void LogHyperlinks(this ILogger logger, string hyperlinks)
    {
        if (logger.IsEnabled(LogLevel.Information))
        {
            LogHyperlinksAction(logger, hyperlinks, default);
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