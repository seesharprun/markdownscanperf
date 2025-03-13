HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Services.AddScoped<IResultsService, MarkdownResultsService>();

builder.Services.AddScoped<MarkdigScanService>();
builder.Services.AddScoped<RegexScanService>();

builder.Services.AddGitHubActionsCore();

builder.Services.AddHostedService<ApplicationWorkerService>();

Option<string> pathOption = new(
    name: "--path",
    description: "The path to the file or directory to scan."
)
{
    IsRequired = true,
    Arity = ArgumentArity.ExactlyOne
};

Option<Tool> toolOption = new(
    name: "--tool",
    description: "The tool to use for scanning."
)
{
    IsRequired = true,
    Arity = ArgumentArity.ExactlyOne
};

Option<bool> outputHyperlinksOption = new(
    name: "--output-hyperlinks",
    description: "Output the hyperlinks found in the markdown files.",
    getDefaultValue: () => false
)
{
    IsRequired = false,
    Arity = ArgumentArity.ExactlyOne,
};

RootCommand command =
[
    pathOption,
    toolOption,
    outputHyperlinksOption
];

command.SetHandler(async (path, tool, outputHyperlinks) =>
{
    builder.Services.AddSingleton(new Configuration()
    {
        Path = path,
        Tool = tool,
        OutputHyperlinks = outputHyperlinks
    });

    builder.Services.AddScoped<IScanService>((provider) =>
    {
        return tool switch
        {
            Tool.Markdig => provider.GetRequiredService<MarkdigScanService>(),
            Tool.Regex => provider.GetRequiredService<RegexScanService>(),
            _ => throw new NotImplementedException($"The selected tool ({tool}) is not implemented.")
        };
    });

    IHost host = builder.Build();

    await host.RunAsync();
}, pathOption, toolOption, outputHyperlinksOption);

await command.InvokeAsync(args);

