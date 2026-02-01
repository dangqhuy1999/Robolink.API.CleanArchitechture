# Sử dụng SDK để build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy tất cả file .csproj và restore (để tận dụng cache)
COPY ["Robolink.WebApp/Robolink.WebApp.csproj", "Robolink.WebApp/"]
COPY ["Robolink.Application/Robolink.Application.csproj", "Robolink.Application/"]
COPY ["Robolink.Infrastructure/Robolink.Infrastructure.csproj", "Robolink.Infrastructure/"]
COPY ["Robolink.Core/Robolink.Core.csproj", "Robolink.Core/"]

RUN dotnet restore "Robolink.WebApp/Robolink.WebApp.csproj"

# Copy toàn bộ code và build
COPY . .
WORKDIR "/src/Robolink.WebApp"
RUN dotnet publish "Robolink.WebApp.csproj" -c Release -o /app/publish

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Robolink.WebApp.dll"]