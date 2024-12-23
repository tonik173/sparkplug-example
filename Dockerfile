FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# copies source folder into app folder
COPY ./src/ ./src

WORKDIR "/app/src/SimulationHost"
RUN dotnet restore ./SimulationHost.csproj -s https://api.nuget.org/v3/index.json

# Build and publish a release
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/src/SimulationHost/out /app
# ENTRYPOINT ["dotnet", "SimulationHost.dll"]
CMD ["arg0", "arg1", "arg2", "arg3", "arg4", "arg5"]