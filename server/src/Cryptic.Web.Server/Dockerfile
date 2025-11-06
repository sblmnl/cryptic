FROM node:22 AS node


FROM mcr.microsoft.com/dotnet/sdk:9.0 AS publish
COPY --from=node . .
WORKDIR /src

COPY client/ client/
COPY .editorconfig .editorconfig
COPY .globalconfig .globalconfig
COPY Cryptic.Web.sln Cryptic.Web.sln
COPY Directory.Build.props Directory.Build.props
COPY Directory.Packages.props Directory.Packages.props
COPY server/src/Cryptic.Core/ server/Cryptic.Core/
COPY server/src/Cryptic.Web.Server/ server/Cryptic.Web.Server/

RUN dotnet publish "server/Cryptic.Web.Server" -c Release -o /app/publish


FROM mcr.microsoft.com/dotnet/aspnet:9.0
USER app
WORKDIR /app

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Cryptic.Web.Server.dll"]
