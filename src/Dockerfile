FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Get_IPlayer_API.csproj", "./"]
RUN dotnet restore "Get_IPlayer_API.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "Get_IPlayer_API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Get_IPlayer_API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR ~/

RUN apt update && apt install wget curl gpg ffmpeg -y
RUN echo 'deb http://download.opensuse.org/repositories/home:/m-grant-prg/Debian_10/ /' | tee /etc/apt/sources.list.d/home:m-grant-prg.list
RUN curl -fsSL https://download.opensuse.org/repositories/home:m-grant-prg/Debian_10/Release.key | gpg --dearmor | tee /etc/apt/trusted.gpg.d/home_m-grant-prg.gpg > /dev/null
RUN apt update && apt install get-iplayer -y

WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Get_IPlayer_API.dll"]
