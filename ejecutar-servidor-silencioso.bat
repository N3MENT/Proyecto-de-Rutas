@echo off
title OptimiRutas - Servidor (silencioso)
set PATH=C:\Users\diego\AppData\Local\Microsoft\dotnet;%PATH%
cd /d "C:\Users\diego\OneDrive\Documentos\Proyecto-de-Rutas"

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
