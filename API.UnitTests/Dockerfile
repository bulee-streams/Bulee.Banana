FROM mcr.microsoft.com/dotnet/core/runtime:2.2-stretch-slim AS base
WORKDIR /app


FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["../API.UnitTests/API.UnitTests.csproj", "../API.UnitTests/"]
COPY ["API/API.csproj", "API/"]
RUN dotnet restore "../API.UnitTests/API.UnitTests.csproj"
COPY . .
WORKDIR "/src/../API.UnitTests"
RUN dotnet build "API.UnitTests.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "API.UnitTests.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "API.UnitTests.dll"]