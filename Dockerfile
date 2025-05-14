# Use official .NET SDK image for building the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy csproj and restore as separate layers
COPY *.csproj ./
RUN dotnet restore

# Copy all files and build the release
COPY . ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out ./

# Expose default port
EXPOSE 80

# Start app
ENTRYPOINT ["dotnet", "smartmetercms.dll"]
