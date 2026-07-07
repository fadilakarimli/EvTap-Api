# EvTap — Project Conventions

EvTap is a rental apartment listings platform backend (portfolio project). Users post listings,
other users save search filters ("saved searches"), and when a new matching listing is approved,
they receive an email + real-time SignalR notification.

## Architecture (non-negotiable)

- .NET 10 (target framework `net10.0` across every project — see "Target framework" note below),
  Modular Monolith with Vertical Slice Architecture and CQRS via MediatR.
- **No controllers** — Minimal API endpoints registered via Carter (one `ICarterModule` per feature slice).
- **No repository pattern, no service classes** — MediatR handlers use the module's own `DbContext` directly.
- EF Core + PostgreSQL — each module has its **own** `DbContext` and its **own** Postgres schema
  (`users`, `listings`, `savedsearches`, `notifications`).
- **Modules never reference each other's projects directly.** Cross-module communication only via
  integration events published with MassTransit + RabbitMQ, using MassTransit's EF Core
  Transactional Outbox on the publishing side.
- Integration event contracts live in `EvTap.Shared.Contracts` (records only, no logic, no
  dependency on `EvTap.Shared`).
- .NET Aspire AppHost orchestrates Postgres, Redis, RabbitMQ, and the API.
- HybridCache (in-memory L1 + Redis L2) for listing search queries.
- SignalR hub for real-time user notifications.
- OpenTelemetry (traces + metrics + logs, exported to the Aspire Dashboard) and Serilog structured logging.
- JWT authentication with roles `User` and `Admin`.
- FluentValidation executed through a MediatR `ValidationBehavior` pipeline, plus a `LoggingBehavior`.
- Result pattern (`Result` / `Result<T>` in `EvTap.Shared`) — no exceptions for business-rule
  failures; endpoints map results to HTTP status codes via `ResultExtensions` in `EvTap.Shared`.
- xUnit + Testcontainers (real Postgres + RabbitMQ) for integration tests.

### Target framework note

The brief originally specified .NET 9, but this machine only has the .NET 10 SDK/runtime
installed (no .NET 9 runtime). By explicit decision, every project in this solution targets
`net10.0` instead. If .NET 9 is ever required (e.g. matching a specific deployment target),
all `<TargetFramework>` entries across every `.csproj` need to change together.

## Solution structure

```
EvTap.slnx
src/
  EvTap.AppHost/                  # Aspire orchestration (AppHost.cs, not Program.cs)
  EvTap.ServiceDefaults/          # Aspire service defaults (OTel, health checks)
  EvTap.Api/                      # Host: composition root, Carter, auth, SignalR hub mapping
  EvTap.Shared/                   # Result, behaviors, abstractions, endpoint helpers
  EvTap.Shared.Contracts/         # Integration event records (no deps on EvTap.Shared)
  Modules/
    EvTap.Modules.Users/
    EvTap.Modules.Listings/
    EvTap.Modules.SavedSearches/
    EvTap.Modules.Notifications/
tests/
  EvTap.IntegrationTests/         # Testcontainers: Postgres + RabbitMQ
```

Each module has (as features land): `Features/<FeatureName>/` (command/query + handler +
validator + endpoint together), `Domain/`, `Persistence/` (DbContext + `IEntityTypeConfiguration<T>`
+ migrations), and a `<ModuleName>Module.cs` with an `Add<ModuleName>Module(IServiceCollection,
IConfiguration)` extension method as the DI entry point.

## Domain model

- **User**: Id (Guid), Name, Email (unique), PasswordHash, Role, CreatedAt.
- **Listing**: Id, Title, Description, Price (decimal), Rooms (int), AreaSquareMeters, District,
  Address, Latitude, Longitude, Status (Pending/Active/Rented/Removed), OwnerId, CreatedAt, ViewCount.
- **ListingImage**: Id, ListingId, Url, Order.
- **Favorite**: UserId + ListingId (composite key).
- **SavedSearch**: Id, UserId, MinPrice?, MaxPrice?, MinRooms?, District?, IsActive, CreatedAt.
- **Notification**: Id, UserId, ListingId, Channel (Email/SignalR), SentAt.

Modules must not share entities. The Notifications module stores only the IDs it needs from
events; SavedSearch matching happens inside the Notifications consumer via a MediatR query
against the SavedSearches module's read model, issued in-process (documented trade-off — the
simpler alternative to a local read-model copy).

## Key flow (showcase feature)

1. `POST /api/listings` → listing created with `Pending` status.
2. Admin calls `POST /api/listings/{id}/approve` → status set to `Active` **and**
   `ListingApprovedEvent` written to the outbox in the same transaction.
3. MassTransit relays the event to RabbitMQ.
4. `ListingApprovedConsumer` in the Notifications module: finds matching active saved searches →
   sends email via MailKit (log-only sender in dev) + pushes via SignalR to online users → writes
   `Notification` rows. Idempotent: never notify the same user twice for the same listing.

## Conventions

- Commands/queries are `sealed record`s; handlers are `internal sealed class`es.
- One folder per feature; all files for a slice live together.
- Endpoints: Carter `ICarterModule` per feature, route groups per module (`/api/listings`,
  `/api/users`, ...), `.RequireAuthorization()` where appropriate.
- EF configurations via `IEntityTypeConfiguration<T>`; each `DbContext` uses `HasDefaultSchema`.
- Pagination: `PagedResult<T>` in `EvTap.Shared`; `GetListings` supports `minPrice, maxPrice, rooms,
  district, sortBy, page, pageSize`.
- Cache key for `GetListings` derived from filter params; short TTL (60s) rather than explicit
  invalidation, for simplicity.
- Swagger/OpenAPI enabled with JWT bearer support.
- Seed one admin user on startup in Development.
- appsettings: JWT secret, SMTP settings; all infra connection strings come from Aspire.
- Result → HTTP mapping: `Result`/`Result<T>` extensions (`ToHttpResult`, `ToCreatedResult`,
  `ToProblemDetails`) live in `EvTap.Shared.Endpoints.ResultExtensions` — endpoints call these
  instead of building `IResult` by hand.
- MediatR pipeline behaviors (`LoggingBehavior`, `ValidationBehavior`) are registered once via
  `AddSharedPipelineBehaviors()` in the Api composition root; each module registers its own
  MediatR handlers/validators from its own assembly in its `Add<ModuleName>Module` method.
- Validators are `internal sealed class`es (matching handlers). `AddValidatorsFromAssembly` does
  **not** find internal validators unless called with `includeInternalTypes: true` — every
  `Add<ModuleName>Module` must pass that flag, or validation silently no-ops (found the hard way:
  invalid input reached the handler and hit the database with zero validation errors).
- `EvTap.Api/wwwroot/` must physically exist in the repo (even just an empty `uploads/.gitkeep`).
  If it's missing, `IWebHostEnvironment.WebRootPath` is `null` at startup and any handler that does
  `Path.Combine(environment.WebRootPath, ...)` (e.g. `UploadListingImagesHandler`) throws
  `ArgumentNullException` — found by actually uploading a file, not just building. `app.UseStaticFiles()`
  is required in `Program.cs` for uploaded images to be servable back at `/uploads/<file>`.
- Every module that has both `Carter` and `Microsoft.EntityFrameworkCore.Design` referenced (i.e.
  every module with a DbContext) picks up the Roslyn/EFCore version-conflict pins automatically from
  `src/Modules/Directory.Build.props` — do not re-add those `Microsoft.CodeAnalysis.*` /
  `Microsoft.Build.Tasks.Core` / `System.Security.Cryptography.Xml` packages per-project.

## Workflow

Build in phases; after each phase, run `dotnet build` (and tests where they exist), fix all
errors/warnings, then stop for confirmation before continuing to the next phase.

1. Solution skeleton, all projects + references, Aspire AppHost, ServiceDefaults, Shared, empty
   module registration, Carter wired, Swagger, Serilog, OTel.
2. Users module — register/login, JWT, roles, migrations, admin seed.
3. Listings module — all features except Approve; HybridCache on GetListings; image upload to
   local `wwwroot/uploads`.
4. SavedSearches module.
5. MassTransit + RabbitMQ + EF Outbox; ApproveListing; Notifications module (consumer, MailKit,
   SignalR, idempotency).
6. Integration tests with Testcontainers; README with mermaid architecture diagram, setup
   instructions, endpoint list.

## Environment notes

- Docker Desktop (with WSL2 backend) is installed as of Phase 2. Postgres/Redis/RabbitMQ
  containers start correctly when the AppHost runs (`docker ps` confirms them), AppHost dashboard
  shows all resources green/Running, and `api` starts successfully end-to-end.
- The AppHost originally pinned `Aspire.Hosting.AppHost`/`Aspire.AppHost.Sdk` etc. at `9.5.2`
  (what the Aspire templates installed with at the time). That version had a real DCP bug on this
  machine — a `dcp-notify-sock ... An invalid argument was supplied` socket error that broke both
  health-check reporting (containers showed false "Unhealthy") and env var/config injection into
  the `api` project resource (it would hang with ~0 CPU forever, or never even launch while
  `.WaitFor(...)` was in play). **Fixed by upgrading to `13.4.6`** (the current stable line) across
  `Aspire.AppHost.Sdk`, `Aspire.Hosting.AppHost`, `Aspire.Hosting.PostgreSQL`, `Aspire.Hosting.Redis`,
  `Aspire.Hosting.RabbitMQ`, and `Microsoft.Extensions.ServiceDiscovery` (→ `10.7.0`) — the socket
  error disappeared entirely and health checks became accurate.
- Separate, real bug found after the upgrade: `AppHost.cs` uses `WithDataVolume()` +
  `ContainerLifetime.Persistent` on Postgres/RabbitMQ so data survives across AppHost restarts —
  but Aspire generates a **fresh random password every launch** by default, while Postgres/RabbitMQ
  only apply `POSTGRES_PASSWORD`/`RABBITMQ_DEFAULT_PASS` on first init of a data directory. A
  persisted volume + a new random password each run caused a repeating password-auth-failure loop
  (visible via `docker logs postgres-e1aa0f66 | grep "password authentication failed"`), which is
  what actually produced the "Unhealthy" status (not a DCP bug on its own this time). **Fixed** by
  pinning fixed passwords via `builder.AddParameter("postgres-password", value: "...", secret: true)`
  (same pattern for RabbitMQ) so the password stays stable across restarts and matches what's baked
  into the persisted volume. If the persisted volume was created under a different image major
  version (e.g. Aspire bumped its default Postgres image tag from `17.6` to `18.3` when 9.5.2→13.4.6
  landed), Postgres refuses to start against it (`Exited (1)`, logs mention pg_upgrade) — delete the
  named volume (`docker volume ls`, `docker volume rm evtap.apphost-<hash>-postgres-data`) and let
  it reinitialize.
- `.WaitFor(evtapDb)` / `.WaitFor(redis)` / `.WaitFor(rabbitmq)` on the `api` project **are required**
  now that health checks are accurate — without them, `api` starts immediately and can lose the race
  against Postgres still initializing (`Npgsql.NpgsqlException: Exception while reading from stream`
  during the startup migration). Do not remove them again.
