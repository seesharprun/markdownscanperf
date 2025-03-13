namespace Examples.MarkdownLinkScanWorker.Interfaces;

internal interface IResultsService
{
    Task SaveResultsAsync(IList<string> hyperlinks, TimeSpan elapsedTime);
}