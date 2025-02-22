#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-nanoserver-1903 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-nanoserver-1903 AS build
WORKDIR /src
COPY ["Web.UI/Web.UI.csproj", "Web.UI/"]
COPY ["Web.ApiControllers/Web.ApiControllers.csproj", "Web.ApiControllers/"]
COPY ["WebFramework/WebFramework.csproj", "WebFramework/"]
COPY ["Services/Services.csproj", "Services/"]
COPY ["Core.Data/Core.Data.csproj", "Core.Data/"]
COPY ["Core.Entities/Core.Entities.csproj", "Core.Entities/"]
COPY ["Common/Common.csproj", "Common/"]
RUN dotnet restore "Web.UI/Web.UI.csproj"
COPY . .
WORKDIR "/src/Web.UI"
RUN dotnet build "Web.UI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Web.UI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Web.UI.dll"]