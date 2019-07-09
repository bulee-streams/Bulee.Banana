FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["API/API.csproj", "API/"]
COPY ["API.UnitTests/API.UnitTests.csproj", "API.UnitTests/"]
COPY ["API.IntegrationTests/API.IntegrationTests.csproj", "API.IntegrationTests/"]

RUN dotnet restore "API/API.csproj"
RUN dotnet restore "API.UnitTests/API.UnitTests.csproj"
RUN dotnet restore "API.IntegrationTests/API.IntegrationTests.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "API/API.csproj" -c Release -o /app
RUN dotnet build "API.UnitTests/API.UnitTests.csproj" -c Release -o /app
RUN dotnet build "API.IntegrationTests/API.IntegrationTests.csproj" -c Release -o /app

RUN dotnet test "API.UnitTests/API.UnitTests.csproj" --logger "trx;LogFileName=webapplication1.trx" 
RUN dotnet test "API.IntegrationTests/API.IntegrationTests.csproj" --logger "trx;LogFileName=webapplication1.trx" 

FROM build AS publish
RUN dotnet publish "API.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "API.dll"]
