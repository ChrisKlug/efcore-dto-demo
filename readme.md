# Demo code for the "Stop using Entity Framework as a DTO provider!" talk

This repo contains the code used for the demos in my talk "Stop using Entity Framework as a DTO provider!". 

The talk is about usingthe features in EF Core to properly map entities instead of just using it to read data from the database.

## Running the code

The code is built around using tests to verify that everything works. This means, that to "run" the code, you just run the tests.

However, the tests use a database. So, you will need to set up a SQL Server instance and make sure that the connection string in the `appSettings.json` files in the different test projects are correct.

## Generating SQL scripts to run during deployment

In my talk, I show how to generate idempotent scripts from the migrations. This makes it possible to run the migrations as part of the deployment, instead of during application startup.

__Comment:__ Running them as part of the startup might seem like a good idea. But it often isn't... For example, in a load balanced environment, you might run into startup issues...

To generate the SQL scripts, just run

```bash
dotnet ef migrations script --idempotent --context MigrationContext --output ./migrations.sql
```

in the directory that contains the project that has the context to be migrated, and the migrations.

__Note.__ In this solution, that means the _/src/EFCore.Domain_.

### Migrations

In this demo, I use multiple `DbContext`'s to allow me to map the same table in different ways. This unfortunately means that I cannot use automated migration creation. However, that is not a big problem, as it is quite easy to create migrations manually.

__Comment:__ I have included a snippet to simplify this. It is available in [./snippets/ef-core-snippets.snippet](./snippets/ef-core-snippets.snippet). Just put it in _C:\Users\<USERNAME>\Documents\Visual Studio 2022\Code Snippets\Visual C#\My Code Snippets_ and it should pop up in Visual Studio and be available as `efmigration`.

However, because I use multiple `DbContext`, there isn't really a `DbContext` to "attach" my migrations to. So, instead, I use a separate dummy context for the migrations, as it is only there to allow the EF Core tools to find the migrations.

One common problem that people tend to face, is that the EF Core tools want a runnable application to be able to locate the context. It also tends to require a database connection, which isn't generally available in a build pipeline.

To solve this, I am using an `IDesignTimeDbContextFactory<T>`. This allows us to configure the context for "design time" actions. In this case, I configure it for SQL Server, but set the connection string to `null`. This means that the EF Core tools will not attempt to communicate with the database when used.

## Generating migration bundles

In the talk, I also cover the new migration bundle feature in EF Core. This allows us to create an executable that is able to migrate the database without us having to connect to it, and run the scripts manually. Something that can be a bit cumbersome in a deployment pipeline.

To generate the bundles, you just run

```bash
dotnet ef migrations bundle --context MigrationContext --output ./migrations.exe
```

in the directory that contains the project that has the context to be migrated, and the migrations.

And to execute the migrations, you just run

```bash
.\migrations.exe --connection "<CONNECTIONSTRING>"
```

## More information

A bit of the content of the talk can be found as a blog post at https://www.fearofoblivion.com/dont-let-ef-call-the-shots

## Contact

If you have any questions, feel free to ping me on Twitter [@ZeroKoll](https://twitter.com/zerokoll). Or wherever you might find me online or IRL...