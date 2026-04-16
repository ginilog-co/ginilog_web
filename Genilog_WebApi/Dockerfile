# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY Genilog_WebApi.csproj ./
RUN dotnet restore Genilog_WebApi.csproj

# Copy entire project and build
COPY . ./
RUN dotnet build Genilog_WebApi.csproj -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish Genilog_WebApi.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Create directory for Firebase credentials
RUN mkdir -p /app/Key

# Copy published files
COPY --from=publish /app/publish .

# Expose port (Render uses 10000 by default)
EXPOSE 10000

# Set environment
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://0.0.0.0:10000

# Install curl for health check
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=40s --retries=3 \
  CMD curl -f http://localhost:10000/api/Health || exit 1

ENTRYPOINT ["dotnet", "Genilog_WebApi.dll"]
