namespace Examples.MarkdownLinkScanWorker.Interfaces;

internal interface IScanService
{
    IAsyncEnumerable<string> GetHyperlinksAsync();
}
