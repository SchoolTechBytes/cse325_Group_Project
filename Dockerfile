# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
# Copy project files first so the restore layer is cached.
COPY src/WheresOurDrummer.Web/WheresOurDrummer.Web.csproj src/WheresOurDrummer.Web/
RUN dotnet restore src/WheresOurDrummer.Web/WheresOurDrummer.Web.csproj
COPY src/ src/
RUN dotnet publish src/WheresOurDrummer.Web/WheresOurDrummer.Web.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
ENV ASPNETCORE_ENVIRONMENT=Production
# Render injects PORT; the app binds to it (default 8080). Documented here only.
EXPOSE 8080
COPY --from=build /app/publish .
# The aspnet image ships a non-root "app" user.
USER app
ENTRYPOINT ["dotnet", "WheresOurDrummer.Web.dll"]
