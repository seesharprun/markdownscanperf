FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /source

COPY . ./
RUN dotnet publish src/worker/Examples.MarkdownLinkScanWorker.csproj --configuration Release --output out --no-self-contained

LABEL com.github.actions.name="Markdown link scanner"
LABEL com.github.actions.description="Scans a folder of Markdown files for hyperlinks."

FROM mcr.microsoft.com/dotnet/runtime:9.0

WORKDIR /app

COPY --from=build /source/out .

ENTRYPOINT [ "dotnet", "Examples.MarkdownLinkScanWorker.dll" ]