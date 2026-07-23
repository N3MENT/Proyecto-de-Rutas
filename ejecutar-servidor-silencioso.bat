@echo off
title OptimiRutas - Servidor (silencioso)
cd /d "%~dp0"

where dotnet >nul 2>&1
if %ERRORLEVEL% neq 0 (
    echo [AVISO] .NET SDK no encontrado. Instalando automaticamente...
    powershell -ExecutionPolicy Bypass -File "%~dp0instalar-dotnet.ps1"
    if %ERRORLEVEL% neq 0 (
        echo ERROR: No se pudo instalar .NET SDK. Verifica tu conexion a internet.
        pause
        exit /b 1
    )
)

:: Inicia el servidor en una ventana minimizada
start /min "" dotnet run --project src/Adapters/OptimiRutas.Api

echo ====================================
echo   OptimiRutas - Servidor iniciado
echo ====================================
echo.
echo  El servidor corre en: http://localhost:5052
echo.
echo  Para ver la app abre el navegador en:
echo     http://localhost:5052
echo.
echo  Presiona cualquier tecla para detener el servidor...
echo.
pause >nul

:: Detener el proceso de dotnet
taskkill /f /im dotnet.exe >nul 2>&1

echo Servidor detenido.
pause >nul
