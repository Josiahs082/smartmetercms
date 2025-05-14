# Use .NET 9.0 SDK for build stage (preview)
FROM mcr.microsoft.com/dotnet/sdk:9.0-preview AS build
WORKDIR /app
COPY *.csproj ./
RUN dotnet restore
COPY . ./
RUN dotnet publish -c Release -o out

# Use .NET 9.0 ASP.NET runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0-preview
WORKDIR /app
COPY --from=build /app/out ./
EXPOSE 80
ENTRYPOINT ["dotnet", "smartmetercms.dll"]
