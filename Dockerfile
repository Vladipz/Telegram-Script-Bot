FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src/

COPY ScriptBot.BLL/ScriptBot.BLL.csproj ./ScriptBot.BLL/ScriptBot.BLL.csproj
COPY ScriptBot.API/ScriptBot.API.csproj ./ScriptBot.API/ScriptBot.API.csproj
COPY ScriptBot.DAL/ScriptBot.DAL.csproj ./ScriptBot.DAL/ScriptBot.DAL.csproj
COPY Directory.Build.props ./

RUN dotnet restore "ScriptBot.API/ScriptBot.API.csproj"

COPY ScriptBot.BLL/ ./ScriptBot.BLL/
COPY ScriptBot.API/ ./ScriptBot.API/
COPY ScriptBot.DAL/ ./ScriptBot.DAL/

RUN dotnet publish -c Release -o ./app ScriptBot.API/ --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app/
COPY --from=build /src/app/ .
ENV ASPNETCORE_HTTP_PORTS=5001
ENTRYPOINT [ "dotnet", "ScriptBot.API.dll" ]
