FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["TravelBooking.Web/TravelBooking.Web.csproj", "TravelBooking.Web/"]
COPY ["TravelBooking.Application/TravelBooking.Application.csproj", "TravelBooking.Application/"]
COPY ["TravelBooking.Domain/TravelBooking.Domain.csproj", "TravelBooking.Domain/"]
COPY ["TravelBooking.Infrastructure/TravelBooking.Infrastructure.csproj", "TravelBooking.Infrastructure/"]
RUN dotnet restore "TravelBooking.Web/TravelBooking.Web.csproj"
COPY . .
WORKDIR "/src/TravelBooking.Web"
RUN dotnet build "./TravelBooking.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./TravelBooking.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TravelBooking.Web.dll"]
