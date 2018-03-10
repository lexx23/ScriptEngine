FROM microsoft/dotnet:2.0-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.0-sdk AS build
WORKDIR /src
COPY *.sln ./
COPY ConsoleApp1/EngineTest.csproj ConsoleApp1/
COPY ScriptEngine/ScriptEngine.csproj ScriptEngine/
RUN dotnet restore
COPY . .
WORKDIR /src/ConsoleApp1
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "EngineTest.dll"]
