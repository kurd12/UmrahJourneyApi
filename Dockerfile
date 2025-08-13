# قۆناغی یەکەم: دروستکردنی پرۆژە (Build Stage)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# کۆپیکردنی فایلەکانی پرۆژە
COPY . .
RUN dotnet restore "UmrahJourneyApi.csproj" --disable-parallel
RUN dotnet publish "UmrahJourneyApi.csproj" -c Release -o /app/publish

# قۆناغی دووەم: دروستکردنی وێنەی کۆتایی (Final Stage)
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "UmrahJourneyApi.dll"]
