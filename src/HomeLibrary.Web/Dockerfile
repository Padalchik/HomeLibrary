FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /source

COPY Directory.Build.props ./
COPY src/HomeLibrary.Domain/HomeLibrary.Domain.csproj src/HomeLibrary.Domain/
COPY src/HomeLibrary.Application/HomeLibrary.Application.csproj src/HomeLibrary.Application/
COPY src/HomeLibrary.Infrastructure/HomeLibrary.Infrastructure.csproj src/HomeLibrary.Infrastructure/
COPY src/HomeLibrary.Web/HomeLibrary.Web.csproj src/HomeLibrary.Web/
RUN dotnet restore src/HomeLibrary.Web/HomeLibrary.Web.csproj

COPY src/ src/
RUN dotnet publish src/HomeLibrary.Web/HomeLibrary.Web.csproj \
    --configuration Release \
    --no-restore \
    --output /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
USER $APP_UID
ENTRYPOINT ["dotnet", "HomeLibrary.Web.dll"]
