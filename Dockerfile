FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app


FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-image

RUN curl -fsSL https://deb.nodesource.com/setup_22.x | bash - \
    && apt update \
    && apt install -y nodejs


FROM build-image AS build
WORKDIR /src

COPY src/Cryptic.Core ./Cryptic.Core
COPY src/Cryptic.Web ./Cryptic.Web

WORKDIR /src/Cryptic.Web

RUN dotnet publish "Cryptic.Web.csproj" -c Release -o /app/publish


FROM base AS final
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Cryptic.Web.dll"]
