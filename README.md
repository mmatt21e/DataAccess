# Scale Management — Data Access Layer

A multi-tenant data access layer for a scale management platform, built on
**.NET 8** and **Entity Framework Core 8**. It captures weighing transactions
from every common weighing technology (truck, hopper, conveyor belt, tank,
silo, crane, checkweigh and livestock scales) behind a single, stable schema,
while letting each customer enable only the scale modules they license — with
no code changes.

---

## Contents

- [Getting started — using it in your project](#getting-started--using-it-in-your-project)
  - [1. Reference the projects](#1-reference-the-projects)
  - [2. Choose an EF Core provider](#2-choose-an-ef-core-provider)
  - [3. Register the data access layer](#3-register-the-data-access-layer)
  - [4. Supply the tenant context](#4-supply-the-tenant-context)
  - [5. Create / migrate the database](#5-create--migrate-the-database)
  - [6. Seed a tenant and enable its modules](#6-seed-a-tenant-and-enable-its-modules)
  - [7. Read and write data](#7-read-and-write-data)
  - [8. Turn on auditing](#8-turn-on-auditing)
- [Solution layout](#solution-layout)
- [Entity model](#entity-model)
- [How the key requirements are met](#how-the-key-requirements-are-met)
- [Reference](#reference): [Database initialization](#database-initialization-existence-check--creation) · [Migrations](#migrations) · [Design notes](#design-notes--trade-offs)

---

## Getting started — using it in your project

A complete walkthrough from a clean app to reading/writing weighing data. The
examples assume SQL Server and ASP.NET Core, with notes for console/worker apps.

> **Namespaces used below**
> ```csharp
> using ScaleManagement.Infrastructure;            // AddScaleManagementDataAccess, InitializeScaleManagementDatabaseAsync
> using ScaleManagement.Infrastructure.Persistence; // ScaleManagementDbContext, DatabaseInitializationOptions
> using ScaleManagement.Infrastructure.Tenancy;      // AmbientTenantContext
> using ScaleManagement.Domain.Repositories;         // IUnitOfWork, repository interfaces
> using ScaleManagement.Domain.Entities;             // Tenant, Scale, Transaction, …
> using ScaleManagement.Domain.Enums;                // ScaleType, MeasurementType, …
> using Microsoft.EntityFrameworkCore;               // UseSqlServer, etc.
> ```

### 1. Reference the projects

Add both projects to your solution and reference **Infrastructure** from your
application (it transitively pulls in Domain):

```bash
dotnet sln add src/ScaleManagement.Domain/ScaleManagement.Domain.csproj
dotnet sln add src/ScaleManagement.Infrastructure/ScaleManagement.Infrastructure.csproj

# from your web/console project:
dotnet add reference path/to/src/ScaleManagement.Infrastructure/ScaleManagement.Infrastructure.csproj
```

### 2. Choose an EF Core provider

The Infrastructure project already references the **SQL Server** provider, so no
extra package is needed for SQL Server. To use a different database, add its
provider package to your application project and call the matching `Use…` method
in step 3 — e.g. `dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL`
then `options.UseNpgsql(connectionString)`.

Put the connection string in `appsettings.json`:

```jsonc
{
  "ConnectionStrings": {
    "ScaleManagement": "Server=localhost;Database=ScaleManagement;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

### 3. Register the data access layer

One call registers the `DbContext`, all repositories, the unit of work, the
tenant context, the auditing pipeline and the database initializer. In a minimal
ASP.NET Core `Program.cs`:

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScaleManagementDataAccess(
    options => options.UseSqlServer(
        builder.Configuration.GetConnectionString("ScaleManagement")),
    init =>
    {
        init.CreateIfNotExists = true;       // create the DB on first run
        init.UseMigrations = true;           // use EF migrations (recommended)
        init.ApplyPendingMigrations = true;  // auto-upgrade an existing DB
    });

// your own services, controllers, auth, etc.
builder.Services.AddControllers();

var app = builder.Build();
```

The second argument is optional — `AddScaleManagementDataAccess(o => o.UseSqlServer(cs))`
works on its own and uses the defaults shown above.

### 4. Supply the tenant context

Every tenant-scoped read/write needs to know the current tenant. The library
registers a scoped `AmbientTenantContext`; set it once per request **after
authentication** so queries are filtered and `TenantId`/audit fields are stamped
automatically:

```csharp
app.UseAuthentication();
app.UseAuthorization();

app.Use(async (httpContext, next) =>
{
    var tenantClaim = httpContext.User.FindFirst("tenant_id")?.Value;
    if (Guid.TryParse(tenantClaim, out var tenantId))
    {
        httpContext.RequestServices
            .GetRequiredService<AmbientTenantContext>()
            .Set(
                tenantId,
                userId: httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                userName: httpContext.User.Identity?.Name,
                correlationId: httpContext.TraceIdentifier);
    }

    await next();
});

app.MapControllers();
app.Run();
```

Prefer to integrate with `HttpContext` directly? Implement your own
`ITenantContext` and register it **before** `AddScaleManagementDataAccess` — the
library uses `TryAdd`, so your registration wins.

### 5. Create / migrate the database

Run the initializer once at startup; it checks whether the database exists and
creates/migrates it per the options from step 3:

```csharp
var app = builder.Build();

var result = await app.Services.InitializeScaleManagementDatabaseAsync();
// result.DatabaseAlreadyExisted, result.DatabaseCreated, result.AppliedMigrations
```

For production, generate a migration first so the schema is created via
migrations (see [Migrations](#migrations)). Without migrations the initializer
falls back to `EnsureCreated()`.

### 6. Seed a tenant and enable its modules

`Tenant` and `TenantModule` are **not** tenant-scoped, so create them with **no
tenant set** (the global filter is then inert). Enable only the scale
technologies the customer has licensed:

```csharp
using var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<ScaleManagementDbContext>();

var tenant = new Tenant { Name = "Acme Aggregates", Code = "acme", TimeZoneId = "America/Chicago" };
db.Tenants.Add(tenant);

db.TenantModules.Add(new TenantModule { TenantId = tenant.Id, ScaleType = ScaleType.TruckScale,  IsEnabled = true });
db.TenantModules.Add(new TenantModule { TenantId = tenant.Id, ScaleType = ScaleType.HopperScale, IsEnabled = true });

await db.SaveChangesAsync();
```

> To create tenant-scoped data (scales, materials, trucks, transactions…) from a
> background job, first set the tenant on the scope:
> ```csharp
> scope.ServiceProvider.GetRequiredService<AmbientTenantContext>()
>     .Set(tenant.Id, userId: "system", userName: "Seeder");
> ```
> `TenantId` is then stamped automatically — never set it by hand on scoped rows.

### 7. Read and write data

Inject `IUnitOfWork` (for multi-entity/atomic work) or a single repository
interface anywhere in your app:

```csharp
public sealed class WeighingService
{
    private readonly IUnitOfWork _uow;

    public WeighingService(IUnitOfWork uow) => _uow = uow;

    public async Task<Guid> RegisterTruckScaleAsync(string code, string name, CancellationToken ct)
    {
        var scale = new Scale
        {
            Code = code,
            Name = name,
            ScaleType = ScaleType.TruckScale,
            WeighingMode = WeighingMode.TwoPass,
            Capacity = 80_000m,
            DefaultUnit = UnitOfMeasure.Kilogram,
        };

        await _uow.Scales.AddAsync(scale, ct);
        await _uow.SaveChangesAsync(ct);   // TenantId + provenance stamped here
        return scale.Id;
    }
}
```

Register your service and you're done:

```csharp
builder.Services.AddScoped<WeighingService>();
```

See the fuller weigh-out example under [Usage example](#usage-example), which
shows capturing measurements and using the open-transaction lookups.

### 8. Turn on auditing

Auditing is off until you add an `AuditConfiguration` row for an entity. See
[Enabling auditing for an entity](#enabling-auditing-for-an-entity).

---

## Solution layout

```
ScaleManagement.sln
└── src
    ├── ScaleManagement.Domain          (no infrastructure dependencies)
    │   ├── Common/                      base classes & cross-cutting interfaces
    │   ├── Enums/                        scale types, measurement types, units…
    │   ├── Entities/                     POCO entity models
    │   ├── Abstractions/                 ITenantContext, IClock
    │   └── Repositories/                 repository + unit-of-work contracts
    └── ScaleManagement.Infrastructure   (EF Core implementation)
        ├── Persistence/
        │   ├── ScaleManagementDbContext.cs
        │   ├── Configurations/           IEntityTypeConfiguration per entity
        │   └── Interceptors/             auditing / tenancy save interceptor
        ├── Repositories/                 repository + UoW implementations
        ├── Auditing/                     per-entity audit policy cache
        ├── Tenancy/                      ambient tenant context, system clock
        └── DependencyInjection.cs        single AddScaleManagementDataAccess(...)
```

The **Domain** project is deliberately persistence-ignorant: it has no
package references, so the model and contracts can be unit-tested and reused
without dragging in EF Core. All EF concerns live in **Infrastructure**.

---

## Entity model

| Entity | Purpose |
| --- | --- |
| `Tenant` | Top-level isolation boundary (one SaaS customer / site operator). |
| `TenantModule` | Enables a single `ScaleType` module for a tenant — the data-driven licensing switch. |
| `Scale` | A physical weighing device of a given `ScaleType`. |
| `Transaction` | A weighing event from any scale type. Gross/tare/net are promoted to columns. |
| `TransactionMeasurement` | Open-ended typed readings (flow rate, batch qty, inventory level, count…). |
| `Material` | A weighed commodity/product, with defaults, density and pricing. |
| `Company` | External party: customer, supplier and/or carrier (flags). |
| `Driver` | A driver, optionally affiliated to a company. |
| `Truck` | A vehicle, with optional stored/permanent tare. |
| `User` | A system operator (auth delegated to an external IdP). |
| `Queue` / `QueueEntry` | Ordered waiting line(s) feeding a scale/process. |
| `AuditConfiguration` | Declares **per-entity, per-tenant** whether/how change auditing runs. |
| `AuditLog` | Append-only change history written automatically inside each transaction. |

### Relationship overview

```
Tenant 1───* TenantModule
Tenant 1───* (every tenant-scoped entity, via TenantId)

Company 1───* Driver        Company 1───* Truck
Company 1───* Transaction   Driver  1───* Transaction
Truck   1───* Transaction   Material 1──* Transaction
Scale   1───* Transaction   User    1───* Transaction (operator)
Scale   1───* Queue

Transaction 1───* TransactionMeasurement   (cascade delete)
Queue       1───* QueueEntry                (cascade delete)
QueueEntry  *───1 Transaction (optional, once weighed)
```

All master-data → `Transaction` relationships use `DeleteBehavior.Restrict`:
weighing history is never cascade-deleted when a scale/company/truck is removed
(and soft delete keeps the referenced rows anyway).

---

## How the key requirements are met

### 1. Multi-tenancy (shared database, shared schema)

Every tenant-owned entity derives from `TenantEntityBase`, which implements
`ITenantScoped`. The `DbContext` builds a **global query filter** for each such
type so reads are automatically restricted to the current tenant, and the save
interceptor stamps `TenantId` on insert. The tenant is read from
`ITenantContext` (resolved per request/operation by the host). When no tenant is
set (migrations, admin tooling) the filter is bypassed.

This gives row-level isolation with a single schema — the most scalable and
operationally simple model for a large SaaS deployment — without every query
having to remember to filter.

### 2. Different customers, different scale modules, no code changes

`ScaleType` enumerates the supported weighing technologies. A tenant turns a
module on or off by inserting/updating a `TenantModule` row; module-specific
behaviour is carried in `TenantModule.SettingsJson`. Because module availability
is **data**, onboarding a customer with only truck scales — or only belt scales
— requires no branching logic and no redeploy. Adding a brand-new technology is
a new enum value plus configuration, not a schema rewrite.

### 3. Any measurement type, any scale technology

Common weights (gross/tare/net) are first-class columns on `Transaction` for
fast querying and reporting. Everything else — flow rate, totalised mass, batch
quantities, inventory level, head count, average weight, volume, deviation,
temperature, moisture — is stored uniformly as `TransactionMeasurement` rows
(`MeasurementType` + `Value` + `Unit` + `SequenceNumber`). One transaction shape
therefore serves every module while remaining fully flexible.

### 4. Optional auditing, per entity

Auditing is **opt-in by data**. `AuditConfiguration` rows declare, for an entity
type, whether auditing is enabled and for which operations (insert/update/
delete), whether to capture before/after images, and which properties to exclude
(secrets/PII). A tenant-specific row overrides the global default
(`TenantId == null`); if neither exists, the entity is not audited.

`AuditingSaveChangesInterceptor` consults a cached `IAuditPolicyProvider` on
every `SaveChanges` and writes `AuditLog` entries **inside the same
transaction** as the change, so the trail can never diverge from the data. The
same interceptor also stamps created/modified provenance and converts hard
deletes of soft-deletable entities into flag updates — the three concerns are
ordered deliberately (stamp → snapshot → soft-delete rewrite).

### 5. Testable & maintainable

- Repository + Unit-of-Work **interfaces** live in Domain; implementations are
  injected, so application code depends on abstractions and is easy to mock.
- `IClock` and `ITenantContext` abstractions make time- and tenant-dependent
  behaviour deterministic in tests.
- `IUnitOfWork.ExecuteInTransactionAsync` wraps multi-aggregate operations in a
  resilient (retry-enabled) transaction.

---

## Reference

The sections below are reference detail for the topics introduced in
[Getting started](#getting-started--using-it-in-your-project).

### Registration

```csharp
services.AddScaleManagementDataAccess(options =>
    options.UseSqlServer(connectionString));
```

The host populates the scoped tenant context per request, e.g. from claims:

```csharp
// in middleware / a filter
var ctx = scope.ServiceProvider.GetRequiredService<AmbientTenantContext>();
ctx.Set(tenantId, userId, userName, correlationId);
```

…or register a custom `ITenantContext` (e.g. HttpContext-backed) **before**
calling `AddScaleManagementDataAccess` — it uses `TryAdd`, so your registration
wins.

### Database initialization (existence check & creation)

The layer can check whether its database exists and create it on startup. An
`IDatabaseInitializer` is registered automatically; configure its behaviour via
the optional second argument:

```csharp
services.AddScaleManagementDataAccess(
    options => options.UseSqlServer(connectionString),
    init =>
    {
        init.CreateIfNotExists = true;       // create the DB if missing (default)
        init.UseMigrations = true;           // create/upgrade via EF migrations (default)
        init.ApplyPendingMigrations = true;  // apply pending migrations to an existing DB (default)
    });
```

Then run it once at startup:

```csharp
var result = await app.Services.InitializeScaleManagementDatabaseAsync();
// result.DatabaseAlreadyExisted / result.DatabaseCreated / result.AppliedMigrations
```

You can also inject `IDatabaseInitializer` and call `DatabaseExistsAsync()` /
`InitializeAsync()` directly.

Behaviour:

- **Exists check** uses the provider's `IRelationalDatabaseCreator`, so it
  distinguishes "database missing" from "cannot connect".
- **`UseMigrations = true`** creates/upgrades through `Database.Migrate()`. If no
  migrations exist in the assembly yet, it transparently falls back to
  `EnsureCreated()` so a usable schema is still produced.
- **`UseMigrations = false`** always builds the schema from the model with
  `EnsureCreated()` — convenient for tests/prototypes, but not upgradable with
  migrations later.
- **`CreateIfNotExists = false`** turns off creation entirely; the initializer
  only reports existence.

"Given the correct information" means the configured connection string must
target a server the process is permitted to create databases on.

### Usage example

```csharp
public async Task<Guid> WeighOutAsync(IUnitOfWork uow, Guid scaleId, Guid truckId, CancellationToken ct)
{
    var scale = await uow.Scales.GetByIdAsync(scaleId, ct)
                ?? throw new InvalidOperationException("Unknown scale");

    var open = await uow.Transactions.GetOpenForTruckAsync(truckId, ct);

    var tx = open ?? new Transaction
    {
        TransactionNumber = (await uow.Transactions.GetMaxSequenceAsync(ct) + 1).ToString("D8"),
        ScaleId = scale.Id,
        ScaleType = scale.ScaleType,
        TruckId = truckId,
        WeighingMode = WeighingMode.TwoPass,
        Status = TransactionStatus.Open,
    };

    // …capture weights, add measurements…
    tx.Measurements.Add(new TransactionMeasurement
    {
        MeasurementType = MeasurementType.GrossWeight,
        Value = 32_500m,
        Unit = scale.DefaultUnit,
        CapturedAtUtc = DateTimeOffset.UtcNow,
    });

    if (open is null) await uow.Transactions.AddAsync(tx, ct);
    await uow.SaveChangesAsync(ct);   // TenantId, provenance & audit all applied automatically
    return tx.Id;
}
```

### Migrations

A `DesignTimeDbContextFactory` is provided so the EF tools work without the app
host. Set the connection string via `SCALEMANAGEMENT_CONNECTION`, then:

```bash
dotnet ef migrations add InitialCreate \
  --project src/ScaleManagement.Infrastructure \
  --startup-project src/ScaleManagement.Infrastructure
dotnet ef database update --project src/ScaleManagement.Infrastructure
```

### Enabling auditing for an entity

Insert an `AuditConfiguration` row (global default shown; set `TenantId` for a
tenant-specific override):

```csharp
db.AuditConfigurations.Add(new AuditConfiguration
{
    TenantId = null,                 // global default
    EntityName = nameof(Transaction),
    IsAuditEnabled = true,
    AuditOnInsert = true,
    AuditOnUpdate = true,
    AuditOnDelete = true,
    CaptureOldValues = true,
    CaptureNewValues = true,
    ExcludedProperties = null,       // e.g. "Notes,InternalRef"
});
await db.SaveChangesAsync();

// then refresh the cache so the next save sees it
auditPolicyProvider.Invalidate();
```

---

## Design notes & trade-offs

- **Surrogate keys are client-generated `Guid`s** (`BaseEntity` seeds
  `Guid.NewGuid()`), which makes keys available before `SaveChanges` so audit
  rows can reference them in the same transaction, and suits distributed/offline
  scale terminals. Use sequential GUIDs at the database if index fragmentation
  becomes a concern.
- **Soft delete is the default** for tenant data because of retention/billing
  requirements; `QueryIncludingDeleted()` exposes deleted rows for admin
  recovery while preserving tenant scoping.
- **Hybrid measurement storage** (promoted weight columns + flexible
  measurement rows) balances query performance against open-ended flexibility.
- The audit policy is cached (10-minute TTL) to keep the save path fast; call
  `IAuditPolicyProvider.Invalidate()` after changing `AuditConfiguration`.
