$ErrorActionPreference = "Stop"

$SDK_VERSION = "10.0"
$INSTALL_SCRIPT_URL = "https://dot.net/v1/dotnet-install.ps1"
$TEMP_SCRIPT = "$env:TEMP\dotnet-install.ps1"

Write-Host ""
Write-Host "===================================="
Write-Host "  Instalador automatico .NET SDK"
Write-Host "===================================="
Write-Host ""
Write-Host "Descargando script de instalacion oficial de Microsoft..."

try {
    [Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
    Invoke-WebRequest -Uri $INSTALL_SCRIPT_URL -OutFile $TEMP_SCRIPT -UseBasicParsing
} catch {
    Write-Host "ERROR: No se pudo descargar el script de instalacion." -ForegroundColor Red
    Write-Host "Verifica tu conexion a internet." -ForegroundColor Yellow
    exit 1
}

Write-Host "Instalando .NET SDK $SDK_VERSION (channel $SDK_VERSION)... esto puede tardar varios minutos."
Write-Host ""

$INSTALL_DIR = "$env:LOCALAPPDATA\Microsoft\dotnet"

$ErrorActionPreference = "Continue"
& $TEMP_SCRIPT -Channel $SDK_VERSION -InstallDir $INSTALL_DIR 2>$null
$ErrorActionPreference = "Stop"

$dotnetPath = "$INSTALL_DIR\dotnet.exe"
if (Test-Path $dotnetPath) {
    $currentPath = [Environment]::GetEnvironmentVariable("PATH", "User")
    if ($currentPath -notlike "*$INSTALL_DIR*") {
        [Environment]::SetEnvironmentVariable("PATH", "$currentPath;$INSTALL_DIR", "User")
        $env:PATH = "$env:PATH;$INSTALL_DIR"
        Write-Host "dotnet agregado al PATH del usuario." -ForegroundColor Green
    }
    Write-Host ""
    Write-Host "Instalacion completada exitosamente!" -ForegroundColor Green
    Write-Host ""
} else {
    Write-Host ""
    Write-Host "ERROR: La instalacion fallo. dotnet.exe no se encontro en $INSTALL_DIR" -ForegroundColor Red
    Write-Host "Intenta instalar manualmente desde: https://dotnet.microsoft.com/download" -ForegroundColor Yellow
    Remove-Item $TEMP_SCRIPT -ErrorAction SilentlyContinue
    exit 1
}

Remove-Item $TEMP_SCRIPT -ErrorAction SilentlyContinue

exit 0
