# Base
FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine AS build-env
WORKDIR /app

COPY . ./
RUN dotnet publish ./src/MinioDotNet.API/MinioDotNet.API.csproj -c Release -o out

# Final
FROM mcr.microsoft.com/dotnet/aspnet:5.0-alpine AS final-env
WORKDIR /app
COPY --from=build-env /app/out .
EXPOSE 5000
ENTRYPOINT ["dotnet", "MinioDotNet.API.dll"]
