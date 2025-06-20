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

#ENV ASPNETCORE_URLS=http://+:5215
#ENTRYPOINT ["dotnet", "DisciplineService.dll"]
# Use the official .NET SDK to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# First copy just the project files (more efficient caching)
COPY *.sln .
COPY DisciplineService/*.csproj ./DisciplineService/
COPY DisciplineService.BusinessLogic/*.csproj ./DisciplineService.BusinessLogic/
COPY DisciplineService.Shared/*.csproj ./DisciplineService.Shared/
COPY DisciplineService.DataAccess/*.csproj ./DisciplineService.DataAccess/

# Restore dependencies
RUN dotnet restore

# Copy everything else
COPY . ./

# Publish specific API project (not the whole solution)
RUN dotnet publish "DisciplineService/DisciplineService.API.csproj" -c Release -o out

# Use the ASP.NET runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .

# Environment variables
ENV ASPNETCORE_URLS=http://+:5215
ENV ASPNETCORE_ENVIRONMENT=Production

# Expose ports (5215 for HTTP, 4800 for gRPC)
EXPOSE 5215
EXPOSE 4800

ENTRYPOINT ["dotnet", "DisciplineService.API.dll"]
