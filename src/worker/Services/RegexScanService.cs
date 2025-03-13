namespace Examples.MarkdownLinkScanWorker.Services;

internal sealed class RegexScanService(
    ILogger<RegexScanService> logger,
    Configuration configuration
) : IScanService
{
    private readonly Regex regex = RegexScanServiceExtensions.ContentSearchPattern();

    private readonly GlobOptions globOptions = new GlobOptionsBuilder()
        .WithBasePath(configuration.Path)
        .WithPattern("**/*.md")
        .WithIgnorePattern(".git")
        .Build();

    public async IAsyncEnumerable<string> GetHyperlinksAsync()
    {
        logger.LogMarkdigScanStarted(configuration.Path);

        IAsyncEnumerable<FileInfo> files = globOptions.GetMatchingFileInfosAsync();

        ConcurrentBag<string> content = [];

        await Parallel.ForEachAsync(files, async (file, cancellationToken) =>
        {
            var text = await File.ReadAllTextAsync(file.FullName, cancellationToken);
            content.Add(text);
        });

        ConcurrentBag<string> hyperlinks = [];

        Parallel.ForEach(content, (string content) =>
        {
            var matches = regex.Matches(content);

            foreach (Match match in matches)
            {
                hyperlinks.Add(match.Value.ToLower());
            }
        });

        foreach (string match in hyperlinks)
        {
            yield return match;
        }
    }
}

internal static partial class RegexScanServiceExtensions
{
    [GeneratedRegex(
        pattern: @"http[s]?://[\/\w\d_.\-]+",
        options: RegexOptions.IgnoreCase,
        cultureName: "en-US")]
    public static partial Regex ContentSearchPattern();
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