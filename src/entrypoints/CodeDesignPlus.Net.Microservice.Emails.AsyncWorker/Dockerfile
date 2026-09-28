FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["nuget.config", "nuget.config"]
COPY ["src/domain/CodeDesignPlus.Net.Microservice.Emails.Application/CodeDesignPlus.Net.Microservice.Emails.Application.csproj", "domain/CodeDesignPlus.Net.Microservice.Emails.Application/"]
COPY ["src/domain/CodeDesignPlus.Net.Microservice.Emails.Domain/CodeDesignPlus.Net.Microservice.Emails.Domain.csproj", "domain/CodeDesignPlus.Net.Microservice.Emails.Domain/"]
COPY ["src/domain/CodeDesignPlus.Net.Microservice.Emails.Infrastructure/CodeDesignPlus.Net.Microservice.Emails.Infrastructure.csproj", "domain/CodeDesignPlus.Net.Microservice.Emails.Infrastructure/"]
COPY ["src/entrypoints/CodeDesignPlus.Net.Microservice.Emails.AsyncWorker/CodeDesignPlus.Net.Microservice.Emails.AsyncWorker.csproj", "entrypoints/CodeDesignPlus.Net.Microservice.Emails.AsyncWorker/"]
RUN dotnet restore "./entrypoints/CodeDesignPlus.Net.Microservice.Emails.AsyncWorker/CodeDesignPlus.Net.Microservice.Emails.AsyncWorker.csproj" /p:PublishReadyToRun=true
COPY ["src/domain/", "domain/"]
COPY ["src/entrypoints/CodeDesignPlus.Net.Microservice.Emails.AsyncWorker/", "entrypoints/CodeDesignPlus.Net.Microservice.Emails.AsyncWorker/"]

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
RUN dotnet publish "./entrypoints/CodeDesignPlus.Net.Microservice.Emails.AsyncWorker/CodeDesignPlus.Net.Microservice.Emails.AsyncWorker.csproj" \
    -c "$BUILD_CONFIGURATION" \
    -o /app/publish \
    /p:UseAppHost=false \
    /p:PublishReadyToRun=true

FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS final
WORKDIR /app
EXPOSE 5002
ENV DOTNET_RUNNING_IN_CONTAINER=true \
    DOTNET_GCDynamicAdaptationMode=1 \
    DOTNET_GCConserveMemory=5 \
    DOTNET_GCHeapHardLimitPercent=70
USER app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CodeDesignPlus.Net.Microservice.Emails.AsyncWorker.dll"]
