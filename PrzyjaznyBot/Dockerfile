FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
WORKDIR /PrzyjaznyBot

COPY ./ ./
RUN dotnet publish -c Release

FROM mcr.microsoft.com/dotnet/aspnet:3.1
WORKDIR /PrzyjaznyBot
COPY --from=build-env /PrzyjaznyBot/bin/Release/netcoreapp3.1/publish/ ./
ENTRYPOINT ["dotnet", "PrzyjaznyBot.dll"]