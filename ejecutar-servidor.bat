@echo off
title OptimiRutas - Servidor
echo ====================================
echo   OptimiRutas - Iniciando servidor
echo ====================================
echo.

cd /d "%~dp0"

where dotnet >nul 2>&1
if %ERRORLEVEL% neq 0 (
    echo [AVISO] .NET SDK no encontrado en el sistema.
    echo         Instalando automaticamente...
    echo.
    powershell -ExecutionPolicy Bypass -File "%~dp0instalar-dotnet.ps1"
    if %ERRORLEVEL% neq 0 (
        echo.
        echo ERROR: No se pudo instalar .NET SDK.
        echo Instala manualmente desde: https://dotnet.microsoft.com/download
        echo.
        pause
        exit /b 1
    )
    echo.
    echo .NET SDK instalado correctamente. Continuando...
    echo.
)

echo  Presiona Ctrl+C para detener el servidor.
echo.

dotnet run --project src/Adapters/OptimiRutas.Api

echo.
echo Servidor detenido.
pause
