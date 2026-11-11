# Sistema de Gestión de Reservas de Hoteles (SGRHotel)

## Descripción

Sistema de Gestión de Reservas de Hoteles desarrollado como proyecto académico. Su objetivo es organizar la administración de hoteles, pisos, habitaciones y tipos de habitaciones mediante una aplicación estructurada por capas.

## Arquitectura

El proyecto utiliza una arquitectura tradicional de tres capas:

- **Presentación:** ASP.NET Core MVC. Gestiona la interfaz y la interacción con el usuario.
- **Business Logic:** contiene las reglas de negocio, validaciones y servicios.
- **Data Access:** administra el acceso a SQL Server mediante Entity Framework Core y el patrón Repository.

## Tecnologías utilizadas

- C#
- .NET
- ASP.NET Core MVC
- Entity Framework Core
- SQL Server
- Bootstrap
- Visual Studio

## Módulos de la capa de Presentación

### 1. Gestión de hoteles
- Registrar hoteles.
- Consultar hoteles.
- Actualizar hoteles.
- Activar y desactivar hoteles.

### 2. Gestión de pisos
- Registrar pisos asociados a hoteles.
- Consultar pisos.
- Actualizar números de pisos.
- Activar y desactivar pisos.

### 3. Gestión de habitaciones
- Registrar habitaciones.
- Consultar habitaciones.
- Actualizar sus datos.
- Cambiar su estado operativo.

### 4. Mantenimiento de tipos de habitaciones
- Registrar tipos de habitaciones.
- Consultar tipos.
- Actualizar nombres y características.

## Estructura de la solución

- `HotelReservasapp.WebApp`: capa de Presentación.
- `HotelReservasapp.BusinessLogic`: capa de lógica de negocio.
- `HotelReservasapp.DataAccess`: capa de acceso a datos.
- `HotelReservasapp.API`: proyecto API incluido en la solución.

## Evidencias visuales

En esta sección se incluirán capturas de pantalla de los módulos implementados para facilitar su revisión.

## Autoría

Proyecto académico desarrollado en C# y SQL Server. By Geraldy A. Pérez Tejada