# Database migrations and seeding

The API initializes its SQL Server database automatically during startup. Before
the HTTP pipeline starts accepting requests, `Program.cs` calls:

```csharp
await app.Services.SeedAsync();
```

`Data/DatabaseSeeder.cs` creates a dependency-injection scope, resolves
`ApplicationDbContext`, and calls `Database.MigrateAsync()`. Entity Framework
then:

1. Creates the database when it does not exist.
2. Applies migrations not yet listed in `__EFMigrationsHistory`.
3. Applies the `InsertData`, `UpdateData`, or `DeleteData` operations generated
   from the model's `HasData` declarations.

The operation is repeatable. When all migrations are current, EF does not insert
the seeded rows again.

## Seeded data

`ApplicationDbContext.SeedLookupData()` defines deterministic IDs and values for
these lookup groups:

- `COURSE_LEVEL`
- `COURSE_CATEGORY`
- `QUESTION_TYPE`
- `QUESTION_STATUS`
- `USER_ROLE`
- `USER_STATUS`
- `EXAM_STATUS`
- `PAYMENT_STATUS`
- `ENROLLMENT_STATUS`
- `CONTENT_TYPE`
- `VIDEO_TYPE`
- `FILE_TYPE`
- `BASKET_STATUS`
- `CONTACT_TYPE`
- `CONTACT_STATUS`
- `EXAM_MODE`
- `SERVICE_TYPE`
- `COURSE_ASSIGNMENT_TYPE`

The revenue migration also seeds permission-link records for course assignment,
revenue shares, distributions, settlements, and the assigned-user dashboard.

Only lookup/reference data is seeded. The supplied SQL Server export is a schema
script and contains no `INSERT` statements, so it does not provide users,
students, courses, or other business records to seed.

## Local setup

Configure `ConnectionStrings:DefaultConnection` with user secrets or an
environment-specific configuration file. Do not commit database credentials.

Using .NET user secrets:

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=HelpEmpowermentData;Trusted_Connection=True;TrustServerCertificate=True"
dotnet run
```

Startup succeeds only after migration and seeding complete. A failure is logged
at `Critical` level and stops the application, preventing it from running against
an incompatible database schema.

## Changing seed data

Never generate IDs dynamically inside `HasData`. Keep stable GUIDs so EF can
identify existing seed rows.

After changing `SeedLookupData`, create and review a migration:

```powershell
dotnet ef migrations add DescribeSeedChange
dotnet ef migrations script --idempotent --output migration.sql
dotnet ef database update
```

Commit the migration `.cs`, its `.Designer.cs`, and the updated model snapshot.
The application will apply the new migration on its next startup.

## Existing and production databases

Back up a production database before deploying a migration. The database login
used by the application needs permission to create or alter schema objects when
automatic migration is enabled.

For environments where the application login must not have DDL permissions,
generate the idempotent script above and apply it through the deployment
pipeline. In that setup, remove or feature-gate the startup `SeedAsync()` call.

Do not import the supplied schema export into a database already managed by these
migrations. A manually created schema can conflict with EF if its migration
history does not accurately describe the objects already present.

## Troubleshooting

- **Login or network error:** verify the connection string, SQL Server address,
  credentials, encryption settings, and firewall access.
- **Object already exists:** the schema was probably created outside EF while
  `__EFMigrationsHistory` is missing or incomplete. Reconcile the database and
  migration history before retrying.
- **Duplicate seed key:** check whether a row was manually inserted using one of
  the deterministic seed GUIDs.
- **Startup timeout:** SQL Server migration commands use the configured 120-second
  command timeout and retry transient failures up to 10 times.

To inspect migration state without changing the database:

```powershell
dotnet ef migrations list
```
