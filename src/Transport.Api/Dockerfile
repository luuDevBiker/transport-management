FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000
EXPOSE 5001

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/Transport.Api/Transport.Api.csproj", "src/Transport.Api/"]
COPY ["src/Transport.Application/Transport.Application.csproj", "src/Transport.Application/"]
COPY ["src/Transport.Domain/Transport.Domain.csproj", "src/Transport.Domain/"]
COPY ["src/Transport.Infrastructure/Transport.Infrastructure.csproj", "src/Transport.Infrastructure/"]
RUN dotnet restore "src/Transport.Api/Transport.Api.csproj"
COPY . .
WORKDIR "/src/src/Transport.Api"
RUN dotnet build "Transport.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Transport.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Transport.Api.dll"]

