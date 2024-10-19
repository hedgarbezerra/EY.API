# Acesse https://aka.ms/customizecontainer para saber como personalizar seu contêiner de depuração e como o Visual Studio usa este Dockerfile para criar suas imagens para uma depuração mais rápida.

# Esta fase é usada durante a execução no VS no modo rápido (Padrão para a configuração de Depuração)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# Esta fase é usada para compilar o projeto de serviço
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["EY.API/EY.API.csproj", "EY.API/"]
RUN dotnet restore "./EY.API/EY.API.csproj"
COPY . .
WORKDIR "/src/EY.API"
RUN dotnet build "./EY.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Install Redis
RUN apt-get update && apt-get install -y redis-server

# Install RedisInsight
RUN curl -o redisinsight.deb -L https://download.redisinsight.redislabs.com/latest/linux
RUN apt-get install -y ./redisinsight.deb

# Install Seq
RUN curl -o seq.tar.gz -L https://getseq.net/Download/Seq/Installer/latest/linux
RUN tar -xzf seq.tar.gz -C /opt
RUN mv /opt/seq* /opt/seq

EXPOSE 80 6379 5540 5341

# Esta fase é usada para publicar o projeto de serviço a ser copiado para a fase final
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./EY.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Esta fase é usada na produção ou quando executada no VS no modo normal (padrão quando não está usando a configuração de Depuração)
CMD service redis-server start && /opt/seq/seq run --port=5341 --storage=/opt/seq/storage --authentication=none & redisinsight

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EY.API.dll"]