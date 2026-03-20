FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY exam_Ef_dapper_14_3.csproj .
RUN dotnet restore

COPY . .
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "exam_Ef_dapper_14_3.dll"]