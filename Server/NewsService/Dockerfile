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

#ENV ASPNETCORE_URLS=http://+:5149
#ENTRYPOINT ["dotnet", "NewsService.dll"]

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy solution and project files first for better layer caching
COPY *.sln .
COPY NewsService/*.csproj ./NewsService/
COPY NewsService.BusinessLogic/*.csproj ./NewsService.BusinessLogic/
COPY NewsService.Shared/*.csproj ./NewsService.Shared/
COPY NewsService.DataAccess/*.csproj ./NewsService.DataAccess/

# Restore dependencies
RUN dotnet restore

# Copy everything else
COPY . ./

# Publish specific API project
RUN dotnet publish "NewsService/NewsService.API.csproj" -c Release -o out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .

# Environment variables
ENV ASPNETCORE_URLS=http://+:5149
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ConnectionStrings__Cache=redis
ENV ConnectionStrings__DataBase=Host=news-db;Port=5432;Database=newsdb;Username=postgres;Password=postgres
ENV GrpcDisciplineSettings__Address=http://discipline-service:4800
ENV StaticFiles__Path=/app/wwwroot/images

# Create directory for images
RUN mkdir -p /app/wwwroot/images

# Expose ports
EXPOSE 5149

VOLUME /app/wwwroot/images

ENTRYPOINT ["dotnet", "NewsService.API.dll"]
