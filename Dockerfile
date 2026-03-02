FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY MiniShopee.sln ./
COPY src/MiniShopee/MiniShopee.csproj src/MiniShopee/
RUN dotnet restore MiniShopee.sln

COPY . .
RUN dotnet publish src/MiniShopee/MiniShopee.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://0.0.0.0:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "MiniShopee.dll"]
