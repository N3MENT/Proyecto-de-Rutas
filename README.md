# Proyecto de Rutas

Aplicación .NET 10 para gestionar rutas y optimizar paradas.

## Estructura del proyecto

- `src/Adapters/OptimiRutas.Api`: API Web que expone los casos de uso.
- `src/Adapters/OptimiRutas.Infrastructure`: Adaptadores de infraestructura y repositorio en memoria.
- `src/Core/OptimiRutas.Application`: Casos de uso y puertos de la aplicación.
- `src/Core/OptimiRutas.Domain`: Entidades, servicios de dominio y objetos de valor.

## Cómo ejecutar

```powershell
cd "c:/Users/abrah/OneDrive/Desktop/PROYECTO RUTAS"
dotnet build
cd src/Adapters/OptimiRutas.Api
dotnet run
```

## Repositorio remoto

https://github.com/abrahammgj2704/Proyecto-de-Rutas
