# Clinic Management System

ASP.NET Core MVC clinic system built with a layered architecture (`Web` -> `BLL` -> `DAL` -> `Common`).
It supports core clinic workflows such as doctor management, patient management, appointments, and account/identity flows.

## Table of Contents

- [Overview](#overview)
- [Architecture](#architecture)
- [Project Structure](#project-structure)
- [Tech Stack](#tech-stack)
- [Request Flow](#request-flow)
- [Getting Started](#getting-started)
- [Configuration](#configuration)
- [Database and Migrations](#database-and-migrations)
- [Logging and Error Handling](#logging-and-error-handling)
- [Development Notes](#development-notes)

## Overview

This solution follows separation of concerns:

- `Web`: MVC presentation layer and app startup.
- `BLL`: business logic, DTO mapping, and service interfaces.
- `DAL`: EF Core context, repositories, and data access concerns.
- `Common`: shared enums, interfaces, and result wrappers.

The current startup entry point is `Web/Program.cs`.

## Architecture

### Layered Design

```text
Web (Controllers, Views, Middleware)
  -> BLL (Application Services, DTOs, Mapping)
	  -> DAL (Repositories, DbContext, EF Core Migrations)
		  -> SQL Server

Common is shared across all layers for contracts, enums, and result models.
```

### Responsibilities

- `Web`
  - Configures dependency injection, authentication, middleware, and routing.
  - Hosts MVC controllers/views and user-facing workflows.
- `BLL`
  - Contains use-case focused services (`DoctorService`, `PatientService`, etc.).
  - Maps entities to DTOs and returns standardized `OperationResult` responses.
- `DAL`
  - Contains `ClinicDbContext`, entity configurations, repositories, and `UnitOfWork`.
  - Implements EF Core access patterns and database migrations.
- `Common`
  - Shared contracts (`ICurrentUserService`, `IAuditable`, `ISoftDeletable`), enums, and result types.

## Project Structure

```text
ClinicManagementSystem/
  Web/        # ASP.NET Core MVC app (startup project)
  BLL/        # Business logic services + DTOs
  DAL/        # EF Core context, repositories, migrations
  Common/     # Shared abstractions, enums, operation results
```

## Tech Stack

- .NET 8 (`net8.0`)
- ASP.NET Core MVC
- Entity Framework Core 8 + SQL Server provider
- ASP.NET Core Identity
- AutoMapper
- Serilog

## Request Flow

1. HTTP request enters `Web` (controller/action route).
2. Controller calls a service from `BLL`.
3. Service in `BLL` uses repositories/`UnitOfWork` from `DAL`.
4. `DAL` executes EF Core queries against SQL Server.
5. Service maps entities to DTOs and returns `OperationResult`.
6. `Web` returns view/response.

## Getting Started

### Prerequisites

- .NET SDK 8.0+
- SQL Server LocalDB or SQL Server instance

### Restore and Build

```powershell
dotnet restore
dotnet build
```

### Run the Web App

```powershell
dotnet run --project .\Web\Web.csproj
```

## Configuration

Primary settings are in `Web/appsettings.json`:

- `ConnectionStrings:DefaultConnection`
- `AppSettings:AppointmentSlotMinutes`
- logging levels (`Logging`)

Example connection string currently used:

```json
"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=ClinicSystemDb;Trusted_Connection=True;MultipleActiveResultSets=true;trustServerCertificate=true;"
```

## Database and Migrations

Migrations are maintained in the `DAL` project.
Design-time context creation is handled by `DAL/Context/ClinicDbContextFactory.cs`.

### Add a Migration

```powershell
dotnet ef migrations add <MigrationName> --project .\DAL\DAL.csproj --startup-project .\Web\Web.csproj
```

### Update Database

```powershell
dotnet ef database update --project .\DAL\DAL.csproj --startup-project .\Web\Web.csproj
```

## Logging and Error Handling

- Serilog is configured in `Web/Program.cs`.
- Error logs are written to `Web/Logs/errors-.txt` (daily rolling files).
- Global exception handling is applied via `ExceptionHandlingMiddleware`.

## Development Notes

- Use async data access end-to-end (`await` repository and save operations).
- Keep business rules in `BLL`, not controllers.
- Return consistent `OperationResult` from services.
- Keep DTOs in `BLL/DTOs` and avoid leaking EF entities to `Web`.
- Prefer repository-level filtering before pagination for correctness/performance.

