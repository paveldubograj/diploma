# Use the official .NET SDK to build the app
#FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
#WORKDIR /app

#COPY . ./
#RUN dotnet restore
#RUN dotnet publish -c Release -o out

# Use the ASP.NET runtime image
#FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
#WORKDIR /app
#COPY --from=build /app/out .

#ENV ASPNETCORE_URLS=http://+:5083
#ENTRYPOINT ["dotnet", "MatchService.dll"]

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy solution and project files first for better layer caching
COPY *.sln .
COPY MatchService/*.csproj ./MatchService/
COPY MatchService.BusinessLogic/*.csproj ./MatchService.BusinessLogic/
COPY MatchService.Shared/*.csproj ./MatchService.Shared/
COPY MatchService.DataAccess/*.csproj ./MatchService.DataAccess/

# Restore dependencies
RUN dotnet restore

# Copy everything else
COPY . ./

# Publish specific API project
RUN dotnet publish "MatchService/MatchService.API.csproj" -c Release -o out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .

# Environment variables
ENV ASPNETCORE_URLS=http://+:5083;http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ConnectionStrings__Cache=redis
ENV ConnectionStrings__DataBase=Host=match-db;Port=5432;Database=matchdb;Username=postgres;Password=postgres

# Expose ports (5083 for HTTP, 5000 for gRPC)
EXPOSE 5083
EXPOSE 5000

ENTRYPOINT ["dotnet", "MatchService.API.dll"]
