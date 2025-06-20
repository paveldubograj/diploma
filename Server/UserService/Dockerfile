#FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
#WORKDIR /app

#COPY . ./
#RUN dotnet restore
#RUN dotnet publish -c Release -o out

# Use the ASP.NET runtime image
#FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
#WORKDIR /app
#COPY --from=build /app/out .

#ENV ASPNETCORE_URLS=http://+:5153
#ENTRYPOINT ["dotnet", "UserService.dll"]
# FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
# WORKDIR /app
#
# # Копируем только файлы решения и проектов
# COPY *.sln .
# COPY UserService.API/*.csproj ./UserService.API/
# COPY UserService.BusinessLogic/*.csproj ./UserService.BusinessLogic/
# COPY UserService.Shared/*.csproj ./UserService.Shared/
# COPY UserService.Infrastructure/*.csproj ./UserService.Infrastructure/
#
# # Восстанавливаем зависимости
# RUN dotnet restore
#
# # 3. Восстанавливаем зависимости с очисткой кэша
# #RUN dotnet nuget locals all --clear
# RUN dotnet restore --no-cache
#
# # Копируем весь исходный код
# COPY . ./
#
# # Публикуем конкретный API проект
# #RUN dotnet clean "UserService.API/UserService.API.csproj" -c Release
# RUN dotnet publish "UserService.API/UserService.API.csproj" -c Release -o out
#
# # Финальный образ
# FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
# WORKDIR /app
# COPY --from=build /app/out .
#
# ENV ASPNETCORE_URLS=http://+:5153
# EXPOSE 5153
# EXPOSE 4900
# ENTRYPOINT ["dotnet", "UserService.API.dll"]

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy solution and project files first for better layer caching
COPY *.sln .
COPY UserService.API/*.csproj ./UserService.API/
COPY UserService.BusinessLogic/*.csproj ./UserService.BusinessLogic/
COPY UserService.Shared/*.csproj ./UserService.Shared/
COPY UserService.Infrastructure/*.csproj ./UserService.Infrastructure/

# Restore dependencies (single restore is sufficient)
RUN dotnet nuget locals --clear all
RUN dotnet restore --no-cache

# Copy remaining source code
COPY . ./

# Publish specific API project with self-contained false
RUN dotnet publish "UserService.API/UserService.API.csproj" \
    -c Release \
    -o /app/out \
    --no-restore \
    -p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .

# Environment variables
ENV ASPNETCORE_URLS=http://+:5153;http://+:4900
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ConnectionStrings__DataBase=Host=user-db;Port=5432;Database=userdb;Username=postgres;Password=postgres
ENV StaticFiles__Path=/app/wwwroot/images

# Create directory for images
RUN mkdir -p /app/wwwroot/images

# Expose ports (5153 for HTTP, 4900 for gRPC)
EXPOSE 5153 4900

# Volume for images
VOLUME /app/wwwroot/images

ENTRYPOINT ["dotnet", "UserService.API.dll"]
