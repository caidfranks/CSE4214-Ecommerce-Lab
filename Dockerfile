FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["GameVault.Server/GameVault.Server.csproj", "GameVault.Server/"]
COPY ["GameVault.Shared/GameVault.Shared.csproj", "GameVault.Shared/"]
RUN dotnet restore "GameVault.Server/GameVault.Server.csproj"

COPY . .
WORKDIR /src/GameVault.Server
RUN dotnet build "GameVault.Server.csproj" -c Release -o /app/build

FROM build AS publish
WORKDIR /src/GameVault.Server
RUN dotnet publish "GameVault.Server.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

COPY --from=publish /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "GameVault.Server.dll"]
