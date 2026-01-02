# Entity Framework Commands

To work with the database, you'll need to install the Entity Framework Core tools and run migrations.

## Install EF Core Tools

```bash
dotnet tool install --global dotnet-ef
```

## Create Initial Migration

```bash
dotnet ef migrations add InitialCreate --project src/TransportService.Infrastructure --startup-project src/TransportService.API
```

## Update Database

```bash
dotnet ef database update --project src/TransportService.Infrastructure --startup-project src/TransportService.API
```

## Add New Migration (when schema changes)

```bash
dotnet ef migrations add MigrationName --project src/TransportService.Infrastructure --startup-project src/TransportService.API
```

## Database Context Info

```bash
dotnet ef dbcontext info --project src/TransportService.Infrastructure --startup-project src/TransportService.API
```

## Generate SQL Script

```bash
dotnet ef migrations script --project src/TransportService.Infrastructure --startup-project src/TransportService.API
```