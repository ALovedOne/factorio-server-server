FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["Factorio.csproj", "./"]
RUN dotnet restore "Factorio.csproj"
COPY . .
RUN dotnet build "Factorio.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Factorio.csproj" -c Release -o /app

# install and cache app dependencies
FROM node as nodebuilder
WORKDIR /app
COPY ClientApp/package.json  ClientApp/package-lock.json ./
RUN npm ci
COPY ClientApp/. .
RUN npm run build
RUN ls -l /app
RUN ls -l /app/build
#End Angular build

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
COPY --from=nodebuilder /app/build/. /app/ClientApp/build/
ENTRYPOINT ["dotnet", "Factorio.dll"]
