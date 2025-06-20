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

#ENV ASPNETCORE_URLS=http://+:5276
#ENTRYPOINT ["dotnet", "TournamentService.dll"]

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy solution and project files first for better layer caching
COPY *.sln .
COPY TournamentService/*.csproj ./TournamentService/
COPY TournamentService.BusinessLogic/*.csproj ./TournamentService.BusinessLogic/
COPY TournamentService.Shared/*.csproj ./TournamentService.Shared/
COPY TournamentService.Infrastructure/*.csproj ./TournamentService.Infrastructure/

# Restore dependencies
RUN dotnet restore

# Copy everything else
COPY . ./

# Publish specific API project
RUN dotnet publish "TournamentService/TournamentService.API.csproj" -c Release -o out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .

# Environment variables
ENV ASPNETCORE_URLS=http://+:5276
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ConnectionStrings__Cache=redis
ENV ConnectionStrings__DataBase=Host=tournament-db;Port=5432;Database=tournamentdb;Username=postgres;Password=postgres
ENV StaticFiles__Path=/app/wwwroot/images
ENV GrpcMatchSettings__Address=http://match-service:5000
ENV GrpcDisciplineSettings__Address=http://discipline-service:4800
ENV GrpcUserSettings__Address=http://user-service:4900

# Create directory for images
RUN mkdir -p /app/wwwroot/images

# Expose HTTP port
EXPOSE 5276

# Volume for image storage
VOLUME /app/wwwroot/images

ENTRYPOINT ["dotnet", "TournamentService.API.dll"]
