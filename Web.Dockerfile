FROM mcr.microsoft.com/dotnet/sdk:7.0.100-alpine3.16-amd64 AS build
WORKDIR /src
COPY ["TechDemo.Core/", "TechDemo.Core/"]
COPY ["TechDemo.DTO/", "TechDemo.DTO/"]
COPY ["TechDemo.Data/", "TechDemo.Data/"]
COPY ["TechDemo.Services/", "TechDemo.Services/"]
COPY ["TechDemo.Web/", "TechDemo.Web/"]
COPY ["TechDemo.Tests/", "TechDemo.Tests/"]

RUN dotnet restore "/src/TechDemo.Web/TechDemo.Web.csproj"

RUN dotnet build "/src/TechDemo.Web/TechDemo.Web.csproj" -c Release -o /app/build

FROM build as publish
RUN dotnet publish "TechDemo.Web/TechDemo.Web.csproj" -c Release -o /app/publish


FROM mcr.microsoft.com/dotnet/aspnet:7.0.0-alpine3.16-amd64 AS final

WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TechDemo.Web/TechDemo.Web.dll"]
