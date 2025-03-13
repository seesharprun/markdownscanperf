namespace Examples.MarkdownLinkScanWorker.Interfaces;

internal interface IResultsService
{
    Task SaveResultsAsync(IAsyncEnumerable<string> results, TimeSpan elapsedTime);
}