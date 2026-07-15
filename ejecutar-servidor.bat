@echo off
title OptimiRutas - Servidor
echo ====================================
echo   OptimiRutas - Iniciando servidor
echo ====================================
echo.
echo  Presiona Ctrl+C para detener el servidor.
echo.

set PATH=C:\Users\diego\AppData\Local\Microsoft\dotnet;%PATH%

cd /d "C:\Users\diego\OneDrive\Documentos\Proyecto-de-Rutas"

dotnet run --project src/Adapters/OptimiRutas.Api

echo.
echo Servidor detenido.
pause
