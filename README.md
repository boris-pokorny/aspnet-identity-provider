# Identity Provider Example

## Start local db engine
<code>
docker run --rm --name pg-docker -e POSTGRES_PASSWORD=docker -p 5432:5432 postgres:11
</code>

## Add initial migrations
<code>
dotnet ef migrations add InitialDbMigration -c ApplicationDbContext -o Data/Migrations
</code>

## Seed users
<code>
dotnet run /seed
</code>

## Start server
<code>
dotnet run
</code>

## Unit tests
<code>
dotnet test
</code>
