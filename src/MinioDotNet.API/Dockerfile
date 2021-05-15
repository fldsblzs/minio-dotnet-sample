FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["src/MinioDotNet.API/MinioDotNet.API.csproj", "MinioDotNet.API/"]
RUN dotnet restore "src/MinioDotNet.API/MinioDotNet.API.csproj"
COPY . .
WORKDIR "/src/MinioDotNet.API"
RUN dotnet build "MinioDotNet.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MinioDotNet.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MinioDotNet.API.dll"]
