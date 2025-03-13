namespace Examples.MarkdownLinkScanWorker.Models;

internal sealed record Configuration
{
    public required string Path { get; init; }

    public required Tool Tool { get; init; }
}