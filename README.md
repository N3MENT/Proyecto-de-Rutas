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
o en su caso abrir el archivo ejecutar-servidor.bat y si tiene instalado puede abrir ejecutar-servidor.sh  ambos funcionan 
```

La API se expondrá por defecto en `https://localhost:7138` o `http://localhost:5188` según la configuración del proyecto.

## Endpoints API

### Obtener todas las rutas

- Método: `GET`
- URL: `/api/rutas`
- Respuesta: lista de `RutaDto`

### Obtener ruta por ID

- Método: `GET`
- URL: `/api/rutas/{id}`
- Parámetros:
  - `id`: GUID de la ruta
- Respuesta: `RutaDto`

### Crear una ruta

- Método: `POST`
- URL: `/api/rutas`
- Cuerpo JSON:

```json
{
  "nombre": "Ruta Matutina Centro",
  "repartidor": "Carlos Gómez",
  "paradas": [
    { "calle": "Av. Principal 124 (Supermercado)", "latitud": 150.0, "longitud": 100.0 },
    { "calle": "Calle de la Paz 89 (Panadería)", "latitud": 250.0, "longitud": 400.0 }
  ]
}
```

- Respuesta: `RutaDto` creada

### Completar una parada

- Método: `PATCH`
- URL: `/api/rutas/{rutaId}/paradas/{paradaId}/completar`
- Parámetros:
  - `rutaId`: GUID de la ruta
  - `paradaId`: GUID de la parada
- Respuesta: `RutaDto` actualizado

## DTOs principales

- `RegistrarRutaCommand`
  - `Nombre` (string)
  - `Repartidor` (string)
  - `Paradas` (lista de `CrearParadaDto`)
- `CompletarParadaCommand`
  - `RutaId` (GUID)
  - `ParadaId` (GUID)
- `RutaDto`
  - `Id`, `Nombre`, `Repartidor`, `ProgresoPorcentaje`, `Estadisticas`, `Paradas`
- `ParadaDto`
  - `Id`, `Calle`, `Lat`, `Lng`, `Completada`, `Orden`

## Repositorio remoto

https://github.com/abrahammgj2704/Proyecto-de-Rutas
