FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "API/API.csproj"
WORKDIR "/src/API"
RUN dotnet build "API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
# Разрешить HTTP/2 без TLS для внутренней коммуникации DOTNET_SYSTEM_NET_HTTP_SOCKETSHTTPHANDLER_HTTP2UNENCRYPTEDSUPPORT=true
ENV ASPNETCORE_ENVIRONMENT="Production" \
    ASPNETCORE_URLS="http://+:5187" \
    DOTNET_SYSTEM_NET_HTTP_SOCKETSHTTPHANDLER_HTTP2UNENCRYPTEDSUPPORT=true
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 5187
ENTRYPOINT ["dotnet", "API.dll"]