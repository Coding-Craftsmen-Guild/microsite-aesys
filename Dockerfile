# syntax=docker/dockerfile:1.7

ARG DOTNET_VERSION=10.0

# ---------- client ----------
# Vite + Tailwind v4 build for CSS/JS. Runs in parallel with dotnet restore
# (Docker BuildKit). Output is wwwroot/dist/ which the build stage copies in
# before `dotnet publish`. Copies only the bundle's actual inputs (vite config,
# Client/, Views/, Aesys.Core .cs for Tailwind @source) so unrelated Aesys.Web
# changes don't invalidate the npm layers.
FROM node:22-alpine AS client
WORKDIR /src/Aesys.Web
COPY Aesys.Web/package*.json ./
RUN npm ci --no-audit --no-fund
COPY Aesys.Web/vite.config.ts Aesys.Web/tsconfig.json ./
COPY Aesys.Web/Client ./Client
COPY Aesys.Web/Views ./Views
COPY Aesys.Core/ /src/Aesys.Core/
RUN npm run build

# ---------- build ----------
# Restore + publish in the SDK image.
FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION} AS build
WORKDIR /src

# Restore layer caches on csproj + central package props only.
COPY Directory.Packages.props ./
COPY Aesys.Web/Aesys.Web.csproj ./Aesys.Web/
COPY Aesys.Core/Aesys.Core.csproj ./Aesys.Core/
RUN dotnet restore Aesys.Web/Aesys.Web.csproj

COPY Aesys.Web/ ./Aesys.Web/
COPY Aesys.Core/ ./Aesys.Core/

# Flatten the code-first uSync sources (Aesys.Core/**/*.config) into
# Aesys.Web/uSync/v17/ContentTypes + Dictionary — same as `mise run usync:bundle`.
# Those folders are gitignored build artefacts (also .dockerignore'd), so a clean
# checkout / `COPY Aesys.Web/` above never carries them; regenerating them here keeps
# the shipped doctypes + dictionary in sync with source instead of relying on a
# developer's local tree. `dotnet publish` picks them up via the Web SDK default
# `**/*.config` content glob and ships them under /app/uSync/v17/.
COPY tools/usync-bundle.sh ./tools/
RUN bash tools/usync-bundle.sh

# Bring in the Vite-built client bundle before publish so `dotnet publish`
# bundles wwwroot/dist/ into the final image.
COPY --from=client /src/Aesys.Web/wwwroot/dist ./Aesys.Web/wwwroot/dist

RUN dotnet publish Aesys.Web/Aesys.Web.csproj \
    -c Release \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false

# ---------- dev ----------
# Used by docker-compose.override.yml: source is bind-mounted in and dotnet watch
# reloads on edit. Compose overrides CMD via `command:`; `docker run --target dev` also works.
FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION} AS dev
WORKDIR /src

# The SDK image (unlike aspnet) sets no port/URL, so bind explicitly for dev.
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1 \
    DOTNET_NOLOGO=1 \
    ASPNETCORE_ENVIRONMENT=Development \
    ASPNETCORE_URLS=http://+:8080 \
    DOTNET_USE_POLLING_FILE_WATCHER=1
EXPOSE 8080

# CMD (not ENTRYPOINT) so a compose-level command cleanly replaces it instead of appending.
# `--no-launch-profile` keeps launchSettings.json's HTTPS/applicationUrl from overriding
# the container's ASPNETCORE_URLS (there's no HTTPS dev cert inside the container).
CMD ["dotnet", "watch", "--non-interactive", "--project", "Aesys.Web/Aesys.Web.csproj", "run", "--no-launch-profile"]

# ---------- runtime ----------
# Production image: aspnet base + published output, non-root. The aspnet image already
# EXPOSEs 8080, binds it via ASPNETCORE_HTTP_PORTS, and sets DOTNET_RUNNING_IN_CONTAINER.
FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION} AS runtime
WORKDIR /app

# Pre-create the Umbraco writable paths owned by the non-root app user — compose mounts
# named volumes here, which would otherwise be root-owned and unwritable by the app.
RUN mkdir -p umbraco/Data umbraco/Logs wwwroot/media && chown -R $APP_UID:$APP_UID /app

COPY --from=build --chown=$APP_UID:$APP_UID /app/publish ./

USER $APP_UID
ENV ASPNETCORE_ENVIRONMENT=Production
ENTRYPOINT ["dotnet", "Aesys.Web.dll"]
