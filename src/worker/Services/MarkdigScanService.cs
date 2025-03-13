namespace Examples.MarkdownLinkScanWorker.Services;

internal sealed class MarkdigScanService(
    ILogger<MarkdigScanService> logger,
    Configuration configuration
) : IScanService
{
    readonly MarkdownPipeline pipeline = new MarkdownPipelineBuilder()
        .UsePreciseSourceLocation()
        .UseAdvancedExtensions()
        .Build();

    public async IAsyncEnumerable<string> GetHyperlinksAsync()
    {
        logger.LogMarkdigScanStarted(configuration.Path);

        string folder = configuration.Path;

        if (!Directory.Exists(folder))
        {
            yield break;
        }

        IEnumerable<string> files = Directory.EnumerateFiles(folder, "*.md", SearchOption.AllDirectories);

        await foreach (string match in ProcessFilesAsync(files))
        {
            yield return match;
        }
    }

    private async IAsyncEnumerable<string> ProcessFilesAsync(IEnumerable<string> files)
    {
        foreach (var file in files)
        {
            await foreach (string match in ProcessFileAsync(file))
            {
                yield return match;
            }
        }
    }

    private async IAsyncEnumerable<string> ProcessFileAsync(string file)
    {
        string content = await File.ReadAllTextAsync(file);

        MarkdownDocument markdown = Markdown.Parse(content, pipeline);

        IEnumerable<string> matches = ProcessHyperlinks(markdown);

        foreach (string match in matches)
        {
            yield return match;
        }
    }

    private static IEnumerable<string> ProcessHyperlinks(MarkdownDocument markdown)
    {
        foreach (LinkInline link in markdown.Descendants<LinkInline>())
        {
            if (link.IsImage || link.Url is null)
            {
                continue;
            }

            if (Uri.TryCreate(link.Url, UriKind.Absolute, out _))
            {
                yield return link.Url;
            }
        }

        foreach (AutolinkInline link in markdown.Descendants<AutolinkInline>())
        {
            if (Uri.TryCreate(link.Url, UriKind.Absolute, out _))
            {
                yield return link.Url;
            }
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