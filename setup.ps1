<#
.SYNOPSIS
    Verifica e instala las dependencias necesarias para compilar y ejecutar OptimiRutas.
.DESCRIPTION
    Este script verifica si el .NET SDK 10 esta instalado.
    Si no lo encuentra, lo descarga e instala automaticamente usando el script oficial de Microsoft.
    Luego ejecuta restore, build y tests del proyecto.
.NOTES
    Ejecutar con: powershell -ExecutionPolicy Bypass -File setup.ps1
#>

param(
    [string]$SdkVersion = "10.0.302"
)

$ErrorActionPreference = "Stop"

function Write-Step($message) {
    Write-Host "`n==> $message" -ForegroundColor Cyan
}

function Write-Success($message) {
    Write-Host "    OK: $message" -ForegroundColor Green
}

function Write-WarningMsg($message) {
    Write-Host "    AVISO: $message" -ForegroundColor Yellow
}

# -------------------------------------------------------
# 1. Verificar si dotnet esta disponible
# -------------------------------------------------------
Write-Step "Verificando si .NET SDK esta instalado..."

$dotnetPath = $null
try {
    $dotnetPath = (Get-Command dotnet -ErrorAction SilentlyContinue).Source
} catch {}

if ($dotnetPath) {
    $installedVersion = & dotnet --version 2>&1
    if ($installedVersion -match "^10\.") {
        Write-Success ".NET SDK encontrado: $installedVersion"
    } else {
        Write-WarningMsg "Se encontro .NET SDK $installedVersion, pero se requiere 10.0.x"
        Write-WarningMsg "Se instalara el SDK 10.0 junto a la version existente."
        $needsInstall = $true
    }
} else {
    Write-WarningMsg "No se encontro .NET SDK. Se instalara automaticamente."
    $needsInstall = $true
}

# -------------------------------------------------------
# 2. Instalar .NET SDK si es necesario
# -------------------------------------------------------
if ($needsInstall) {
    Write-Step "Descargando instalador oficial de .NET SDK 10.0..."

    $installScript = Join-Path $env:TEMP "dotnet-install.ps1"
    $url = "https://dot.net/v1/dotnet-install.ps1"

    try {
        Invoke-WebRequest -Uri $url -OutFile $installScript -UseBasicParsing
        Write-Success "Script de instalacion descargado."
    } catch {
        Write-Host "    ERROR: No se pudo descargar el script de instalacion." -ForegroundColor Red
        Write-Host "    Descargalo manualmente desde: https://dot.net/v1/dotnet-install.ps1" -ForegroundColor Red
        exit 1
    }

    Write-Step "Instalando .NET SDK $SdkVersion..."
    & powershell -ExecutionPolicy Bypass -File $installScript -Channel 10.0 -Quality GA

    if ($LASTEXITCODE -ne 0) {
        Write-Host "    ERROR: La instalacion del SDK fallo." -ForegroundColor Red
        exit 1
    }

    # Actualizar PATH para esta sesion
    $dotnetDir = Join-Path $env:LOCALAPPDATA "Microsoft\dotnet"
    $env:PATH = "$dotnetDir;$env:PATH"

    Write-Success ".NET SDK 10.0 instalado correctamente."
}

# -------------------------------------------------------
# 3. Verificar la ruta del proyecto
# -------------------------------------------------------
$projectRoot = Split-Path -Parent $MyInvocation.MyCommand.Definition
if (-not (Test-Path "$projectRoot\OptimiRutas.slnx")) {
    Write-Host "    ERROR: No se encontro OptimiRutas.slnx en $projectRoot" -ForegroundColor Red
    exit 1
}

Write-Step "Restaurando paquetes NuGet..."
& dotnet restore "$projectRoot\OptimiRutas.slnx"
if ($LASTEXITCODE -ne 0) {
    Write-Host "    ERROR: dotnet restore fallo." -ForegroundColor Red
    exit 1
}
Write-Success "Paquetes restaurados."

Write-Step "Compilando el proyecto..."
& dotnet build "$projectRoot\OptimiRutas.slnx" --configuration Release
if ($LASTEXITCODE -ne 0) {
    Write-Host "    ERROR: dotnet build fallo." -ForegroundColor Red
    exit 1
}
Write-Success "Proyecto compilado correctamente."

Write-Step "Ejecutando tests..."
& dotnet test "$projectRoot\OptimiRutas.slnx" --configuration Release --verbosity normal
if ($LASTEXITCODE -ne 0) {
    Write-Host "    ERROR: Algunos tests fallaron." -ForegroundColor Red
    exit 1
}
Write-Success "Todos los tests pasaron."

Write-Host "`n==========================================" -ForegroundColor Green
Write-Host "  Setup completado exitosamente!" -ForegroundColor Green
Write-Host "  Para ejecutar la API:" -ForegroundColor Green
Write-Host "    cd src\Adapters\OptimiRutas.Api" -ForegroundColor Yellow
Write-Host "    dotnet run" -ForegroundColor Yellow
Write-Host "==========================================" -ForegroundColor Green
