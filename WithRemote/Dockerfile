FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

# context points to root folder
COPY ./Shared ./Shared

COPY ./WithRemote/*.csproj ./WithRemote/
RUN dotnet restore WithRemote

COPY ./WithRemote ./WithRemote
RUN dotnet publish WithRemote -o out

# build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app/out .
EXPOSE 80
ENTRYPOINT ["dotnet", "WithRemote.dll"]
