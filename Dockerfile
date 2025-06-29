# Build da aplicação
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copia o csproj e restaura dependências
COPY *.csproj ./
RUN dotnet restore

# Copia todo o restante do código e faz o publish
COPY . ./
RUN dotnet publish ProductAPI.csproj -c Release -o out

# Imagem para rodar a aplicação
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

# Copia a build final da etapa anterior
COPY --from=build /app/out .

# Define a porta que o container vai expor
EXPOSE 80

# Variável para que o app escute em http na porta 5000
ENV ASPNETCORE_URLS=http://+:5000

# Comando para rodar a aplicação
ENTRYPOINT ["dotnet", "ProductAPI.dll"]
