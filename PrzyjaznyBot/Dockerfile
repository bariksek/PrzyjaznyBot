FROM mcr.microsoft.com/dotnet/aspnet:3.1
COPY bin/Release/netcoreapp3.1/publish/ PrzyjaznyBot/
WORKDIR /PrzyjaznyBot
ENTRYPOINT ["dotnet", "PrzyjaznyBot.dll"]