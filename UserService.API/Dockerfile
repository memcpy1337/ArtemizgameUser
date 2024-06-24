# Base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 6033
EXPOSE 6034

# Build image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project files and restore dependencies
COPY ["UserService.API/UserService.API.csproj", "UserService.API/"]
COPY ["UserService.API/serilog.json", "UserService.API/"]
RUN dotnet restore "UserService.API/UserService.API.csproj"
COPY . .
WORKDIR "/src/UserService.API"

RUN dotnet build "UserService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish image
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "UserService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish --no-restore

# Final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UserService.API.Build.dll"]
