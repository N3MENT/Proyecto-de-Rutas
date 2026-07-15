#!/bin/bash
# OptimiRutas - Iniciar servidor

echo "===================================="
echo "  OptimiRutas - Iniciando servidor"
echo "===================================="
echo ""
echo "  Presiona Ctrl+C para detener el servidor."
echo ""

cd "$(dirname "$0")"

dotnet run --project src/Adapters/OptimiRutas.Api

echo ""
echo "Servidor detenido."
