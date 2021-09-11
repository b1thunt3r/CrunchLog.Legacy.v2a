#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["src/Bit0.CrunchLog.Cli/Bit0.CrunchLog.Cli.csproj", "src/Bit0.CrunchLog.Cli/"]
COPY ["src/Bit0.CrunchLog/Bit0.CrunchLog.csproj", "src/Bit0.CrunchLog/"]
COPY ["src/Bit0.CrunchLog.Sdk/Bit0.CrunchLog.Sdk.csproj", "src/Bit0.CrunchLog.Sdk/"]
COPY ["utils/Bit0.Serilog.Sinks.SpectreConsole/Bit0.Serilog.Sinks.SpectreConsole.csproj", "utils/Bit0.Serilog.Sinks.SpectreConsole/"]
COPY . .
RUN dotnet restore "src/Bit0.CrunchLog.Cli/Bit0.CrunchLog.Cli.csproj" --runtime alpine-x64
WORKDIR "/src/src/Bit0.CrunchLog.Cli"
RUN dotnet publish "Bit0.CrunchLog.Cli.csproj" -c Release -o /app/publish --no-restore --runtime alpine-x64 --self-contained true /p:PublishTrimmed=true

FROM mcr.microsoft.com/dotnet/runtime-deps:6.0-alpine AS final
WORKDIR /app
RUN adduser --disabled-password --home /site --gecos '' dotnetuser && chown -R dotnetuser /app
USER dotnetuser
WORKDIR /site
COPY --from=build /app/publish /app/
ENTRYPOINT ["/app/crunch"]