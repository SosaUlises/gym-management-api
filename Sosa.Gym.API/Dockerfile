# 1. Imagen base para compilar
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY *.sln ./
COPY Sosa.Gym.API/Sosa.Gym.API.csproj Sosa.Gym.API/
COPY Sosa.Gym.Application/Sosa.Gym.Application.csproj Sosa.Gym.Application/
COPY Sosa.Gym.Domain/Sosa.Gym.Domain.csproj Sosa.Gym.Domain/
COPY Sosa.Gym.Common/Sosa.Gym.Common.csproj Sosa.Gym.Common/
COPY Sosa.Gym.Infraestructure/Sosa.Gym.Infraestructure.csproj Sosa.Gym.Infraestructure/
COPY Sosa.Gym.UnitTests/Sosa.Gym.UnitTests.csproj Sosa.Gym.UnitTests/

# Restaurar dependencias
RUN dotnet restore

COPY . .

# Compilar y publicar
WORKDIR "/src/Sosa.Gym.API"
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Sosa.Gym.API.dll"]