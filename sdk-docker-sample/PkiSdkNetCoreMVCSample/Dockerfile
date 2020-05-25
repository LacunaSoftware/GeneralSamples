FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-alpine AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-alpine AS build
WORKDIR /src
COPY ["PkiSdkNetCoreMVCSample/PkiSdkNetCoreMVCSample.csproj", "PkiSdkNetCoreMVCSample/"]
RUN dotnet restore "PkiSdkNetCoreMVCSample/PkiSdkNetCoreMVCSample.csproj"
COPY . .
WORKDIR "/src/PkiSdkNetCoreMVCSample"
RUN dotnet build "PkiSdkNetCoreMVCSample.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PkiSdkNetCoreMVCSample.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PkiSdkNetCoreMVCSample.dll"]