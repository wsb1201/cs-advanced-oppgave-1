# ---------------BUILD---------------------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY LibrarySystem.Api/LibrarySystem.Api.csproj LibrarySystem.Api/
COPY LibrarySystem.Domain/LibrarySystem.Domain.csproj LibrarySystem.Domain/
RUN dotnet restore LibrarySystem.Api/LibrarySystem.Api.csproj

COPY LibrarySystem.Api/ LibrarySystem.Api/
COPY LibrarySystem.Domain/ LibrarySystem.Domain/
RUN dotnet publish LibrarySystem.Api/LibrarySystem.Api.csproj \
	-c Release -o /app/publish --no-restore


# -------------RUNTIME---------------
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "LibrarySystem.Api.dll"]
