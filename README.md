# 📦 AssetTracker — Sistema de Gestión de Activos Empresariales

![.NET](https://img.shields.io/badge/.NET-10-512BD4?logo=dotnet)
![C#](https://img.shields.io/badge/C%23-10-239120?logo=csharp)
![SQLite](https://img.shields.io/badge/SQLite-3-003B57?logo=sqlite)
![Bootstrap](https://img.shields.io/badge/Bootstrap-5-7952B3?logo=bootstrap)
![License](https://img.shields.io/badge/License-MIT-green)

Sistema web para la gestión integral de activos físicos y digitales de una organización. Permite el control del ciclo de vida de los activos (alta, depreciación, asignación, mantenimiento y baja), la administración de empleados con roles y permisos diferenciados, y la generación de reportes y estadísticas en tiempo real.

Desarrollado como **Trabajo Práctico Integrador** de la materia **Programación .NET**.

---

## 📑 Tabla de contenidos

- [🎯 Objetivos](#-objetivos)
- [🛠️ Stack tecnológico](#-stack-tecnológico)
- [🏗️ Arquitectura](#-arquitectura)
- [✨ Características principales](#-características-principales)
- [🔐 Roles y permisos (RBAC)](#-roles-y-permisos-rbac)
- [📁 Estructura del proyecto](#-estructura-del-proyecto)
- [🚀 Instalación y configuración](#-instalación-y-configuración)
- [🌐 Endpoints de la API](#-endpoints-de-la-api)
- [🧪 Usuarios de prueba](#-usuarios-de-prueba)
- [🛠️ Refactorización y buenas prácticas](#-refactorización-y-buenas-prácticas)
- [📸 Capturas de pantalla](#-capturas-de-pantalla)
- [👥 Autor](#-autor)
- [📄 Licencia](#-licencia)

---

## 🎯 Objetivos

- Centralizar la información de activos, empleados y asignaciones en una única plataforma.
- Automatizar el cálculo de depreciación lineal de activos físicos y digitales.
- Controlar el acceso a la información mediante un sistema de roles con alcance por departamento.
- Replicar la usabilidad y paridad visual de la aplicación de escritorio de referencia, llevándola a una arquitectura web moderna.
- Aplicar buenas prácticas de ingeniería de software: DRY, SRP, separación de responsabilidades y defensa en profundidad.

---

## 🛠️ Stack tecnológico

### Backend (API)

- **.NET 10** (C# 13)
- **ASP.NET Core Web API**
- **Entity Framework Core** + **SQLite**
- **ASP.NET Core Identity** (autenticación y autorización)
- **Swashbuckle** (Swagger UI para documentación de la API)

### Frontend (Web)

- **ASP.NET Core Razor Pages**
- **Bootstrap 5** + **Bootstrap Icons**
- **JavaScript vanilla** (búsqueda en vivo con debounce)
- **HttpClient** con `IHttpClientFactory` (patrón Facade)

### Herramientas

- **Visual Studio 2022 / VS Code**
- **Git** + **GitHub**
- **DB Browser for SQLite** (para inspección de base de datos)

---

## 🏗️ Arquitectura

El proyecto sigue una **arquitectura cliente-servidor desacoplada**, con dos soluciones independientes que se comunican mediante HTTP/REST:

```text
┌─────────────────────────────────────────────────────────────┐
│                       AssetTracker.Web                      │
│      (Razor Pages + Bootstrap + AssetTrackerApiClient)      │
│  • UI sensible al rol                                       │
│  • Cliente HTTP centralizado (Facade)                       │
│  • Sin acceso directo a la base de datos                    │
└───────────────────────┬─────────────────────────────────────┘
                        │ HTTP/REST (JSON)
                        ▼
┌─────────────────────────────────────────────────────────────┐
│                       AssetTracker.API                      │
│           (Web API + EF Core + Identity + SQLite)           │
│  • Controladores REST                                       │
│  • Repositorios + DbContext                                 │
│  • Validación de dominio                                    │
└─────────────────────────────────────────────────────────────┘
```

### Principios aplicados

- **Separation of Concerns**: la UI no conoce la base de datos; la API no conoce la UI.
- **Single Responsibility**: cada `PageModel` tiene una única responsabilidad.
- **DRY (Don't Repeat Yourself)**: la lógica HTTP vive en un único cliente (`AssetTrackerApiClient`).
- **Defense in Depth**: la autorización se aplica en el servidor (`[Authorize]`) y en la UI (botones ocultos según rol).
- **Single Source of Truth**: roles y catálogos centralizados en clases estáticas (`Permisos`, `CatalogosEmpleados`).

---

## ✨ Características principales

### 📦 Gestión de Activos

- Alta de activos **físicos** (con número de serie y ubicación) y **digitales** (con clave de licencia y vencimiento).
- Códigos internos autogenerados (`ACT-0001`, `ACT-0002`, ...).
- Cambio de estado: `Disponible`, `Asignado`, `En Mantenimiento`, `Dado de Baja`.
- **Depreciación lineal automática** con vida útil configurable.
- Búsqueda global en vivo (con debounce de 300 ms).
- Filtros por estado, ordenamiento y búsqueda por texto.

### 👥 Gestión de Empleados

- Ficha completa: documento, correo, teléfono, cargo, departamento, fecha de ingreso.
- Estados: `Activo` / `Inactivo`.
- Catálogos fijos de cargos y departamentos.
- Eliminación con **alcance por departamento** (solo para `AdminDepartamento`).

### 🔗 Asignaciones

- Asignación de activos disponibles a empleados.
- Liberación automática del activo al eliminar la asignación.
- Historial de asignaciones por empleado.

### 🔧 Mantenimientos

- Mantenimientos **preventivos** y **correctivos**.
- Estados derivados: `Pendiente`, `En Progreso`, `Vencido`, `Completado`, `Cancelado`.
- Filas coloreadas por estado (paridad con el escritorio).
- Acciones rápidas: completar, cancelar, eliminar.

### 📊 Reportes y estadísticas

- Dashboard con KPIs: total de activos, valor total, disponibles, asignados.
- Reporte por estado con porcentajes.
- Barras de estadísticas en todos los listados.
- Cálculo de depreciación acumulada y valor en libros.

---

## 🔐 Roles y permisos (RBAC)

El sistema implementa **Control de Acceso Basado en Roles** con 4 niveles:

| Acción | Empleado | AdminInventario | AdminDepartamento | Administrador |
|---|:---:|:---:|:---:|:---:|
| Ver listados, fichas y reportes | ✅ | ✅ | ✅ | ✅ |
| Crear / editar activos | ❌ | ✅ | ❌ | ✅ |
| Crear / eliminar asignaciones | ❌ | ✅ | ❌ | ✅ |
| Gestionar mantenimientos | ❌ | ✅ | ❌ | ✅ |
| **Eliminar activos** | ❌ | ❌ | ❌ | ✅ |
| Crear / editar empleados | ❌ | ❌ | ✅ (su depto) | ✅ |
| **Eliminar empleados** | ❌ | ❌ | ✅ **solo su depto** | ✅ (todos) |
| Crear usuarios del sistema | ❌ | ❌ | ❌ | ✅ |

### Regla destacada: alcance por departamento

El rol `AdminDepartamento` solo puede eliminar empleados **de su propio departamento**. Por ejemplo, un administrador de TI no puede eliminar un empleado de Ventas. El `Administrador` tiene alcance global.

---

## 📁 Estructura del proyecto

```text
AssetTracker/
├── AssetTracker.API/  # Solución backend
│   ├── Controllers/   # Endpoints REST
│   ├── Data/          # DbContext + migraciones
│   ├── Dto/           # DTOs de entrada/salida
│   ├── Models/        # Entidades de dominio
│   ├── Repositories/  # Acceso a datos
│   └── Program.cs     # Configuración de la API
│
├── AssetTracker.Web/  # Solución frontend
│   ├── DTOs/          # DTOs propios del Web
│   ├── Data/          # ApplicationUser + ApplicationDbContext
│   ├── Helpers/       # Permisos, CatalogosEmpleados, DepreciacionHelper
│   ├── Pages/         # Razor Pages (Activos, Empleados, etc.)
│   ├── Services/      # IAssetTrackerApiClient + implementación
│   ├── ViewModels/    # ViewModels para las vistas
│   ├── wwwroot/       # CSS, JS, imágenes
│   └── Program.cs     # Configuración del Web
│
└── README.md          # Este archivo
```

---

## 🚀 Instalación y configuración

### Requisitos previos

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Git](https://git-scm.com/)
- Editor: Visual Studio 2022, VS Code o JetBrains Rider

### Paso 1: Clonar el repositorio

```bash
git clone https://github.com/tu-usuario/AssetTracker.git
cd AssetTracker
```

### Paso 2: Levantar la API

```bash
cd AssetTracker.API/AssetTracker.API
dotnet restore
dotnet ef database update
dotnet run --launch-profile https
```

La API queda disponible en https://localhost:7087 (Swagger UI incluido).

### Paso 3: Levantar el Web

```bash
cd ../../AssetTracker.Web/AssetTracker.Web
dotnet restore
dotnet ef database update -c ApplicationDbContext
dotnet run
```

El Web queda disponible en http://localhost:5204.

### Paso 4: Iniciar sesión

Usá las credenciales de los [usuarios de prueba](#-usuarios-de-prueba).

---

## 🌐 Endpoints de la API

### Activos

| Método | Ruta | Descripción |
|---|---|---|
| `GET` | `/api/activos` | Listar activos (con filtros) |
| `GET` | `/api/activos/{id}` | Detalle de un activo |
| `GET` | `/api/activos/estados` | Catálogo de estados |
| `GET` | `/api/activos/estadisticas` | KPIs del dashboard |
| `GET` | `/api/activos/reporte` | Reporte por estado |
| `GET` | `/api/activos/buscar?texto=` | Búsqueda global |
| `POST` | `/api/activos` | Crear activo |
| `PUT` | `/api/activos/{id}` | Editar activo |
| `PUT` | `/api/activos/{id}/estado` | Cambiar estado |
| `DELETE` | `/api/activos/{id}` | Eliminar activo |

### Empleados

| Método | Ruta | Descripción |
|---|---|---|
| `GET` | `/api/empleados` | Listar empleados |
| `GET` | `/api/empleados/{id}` | Detalle |
| `GET` | `/api/empleados/{id}/detalle` | Ficha completa con asignaciones |
| `GET` | `/api/empleados/{id}/tiene-asignaciones` | Verificar cascada de baja |
| `POST` | `/api/empleados` | Crear empleado |
| `PUT` | `/api/empleados/{id}` | Editar empleado |
| `DELETE` | `/api/empleados/{id}` | Eliminar empleado |

### Asignaciones

| Método | Ruta | Descripción |
|---|---|---|
| `GET` | `/api/asignaciones` | Listar asignaciones |
| `GET` | `/api/asignaciones/{id}` | Detalle |
| `POST` | `/api/asignaciones` | Crear asignación |
| `DELETE` | `/api/asignaciones/{id}` | Eliminar asignación |

### Mantenimientos

| Método | Ruta | Descripción |
|---|---|---|
| `GET` | `/api/mantenimientos` | Listar (con filtros) |
| `GET` | `/api/mantenimientos/{id}` | Detalle |
| `POST` | `/api/mantenimientos` | Crear mantenimiento |
| `PUT` | `/api/mantenimientos/{id}/estado` | Cambiar estado |
| `DELETE` | `/api/mantenimientos/{id}` | Eliminar |

---

## 🧪 Usuarios de prueba

En modo **Desarrollo**, el sistema siembra automáticamente 4 usuarios al arrancar:

| Email | Contraseña | Rol | Departamento |
|---|---|---|---|
| `admin@assettracker.com` | `Admin123!` | Administrador | — |
| `inventario@assettracker.com` | `Admin123!` | AdminInventario | Inventario |
| `depto@assettracker.com` | `Admin123!` | AdminDepartamento | TI |
| `empleado@assettracker.com` | `Admin123!` | Empleado | Ventas |

> ⚠️ En producción, estos usuarios no se siembran y el registro público está deshabilitado.

---

## 🛠️ Refactorización y buenas prácticas

### Code smells detectados y resueltos

| Code Smell | Solución aplicada |
|---|---|
| Duplicación de código en `PageModel` (creación de `HttpClient`, `try/catch`, deserialización) | Patrón Facade con `AssetTrackerApiClient` centralizado |
| Cadenas mágicas (roles `"Administrador"` dispersos) | Clase estática `Permisos` como Single Source of Truth |
| Registro público de usuarios (riesgo de escalada de privilegios) | Endpoint `/Account/Register` restringido al `Administrador` con lista blanca de roles |
| Contrato JSON inconsistente (PascalCase vs. camelCase) | `JsonSerializerDefaults.Web` en el proxy de búsqueda |

### Principios de diseño aplicados

- **DRY (Don't Repeat Yourself)**: lógica HTTP en un solo lugar.
- **SRP (Single Responsibility Principle)**: cada clase tiene una única responsabilidad.
- **Defense in Depth**: autorización en servidor + UI sensible al rol.
- **Fail-Fast**: validaciones de dominio en los modelos (`Validar()`).
- **Convention over Configuration**: estructura estándar de ASP.NET Core.

---

## 👥 Autor

**Santiago Somma**

Desarrollado como parte del Trabajo Práctico Integrador de la materia Programación .NET.

- 📧 ssomma@iiconcepcion.edu.ar

---

