FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY . .

RUN dotnet restore "ClassroomSync.WebApi/ClassroomSync.WebApi.csproj"
RUN dotnet publish "ClassroomSync.WebApi/ClassroomSync.WebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "ClassroomSync.WebApi.dll"]