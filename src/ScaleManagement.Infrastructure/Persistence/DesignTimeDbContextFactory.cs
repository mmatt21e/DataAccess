using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using ScaleManagement.Infrastructure.Tenancy;

namespace ScaleManagement.Infrastructure.Persistence;

/// <summary>
/// Lets the EF Core tools (<c>dotnet ef migrations add</c> / <c>database update</c>)
/// build the context at design time without the application's DI container. It
/// runs unscoped (no tenant) so global tenant filters are bypassed during schema
/// operations. The connection string is read from the
/// <c>SCALEMANAGEMENT_CONNECTION</c> environment variable, falling back to a
/// local SQL Server instance.
/// </summary>
public sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ScaleManagementDbContext>
{
    public ScaleManagementDbContext CreateDbContext(string[] args)
    {
        var connectionString =
            Environment.GetEnvironmentVariable("SCALEMANAGEMENT_CONNECTION")
            ?? "Server=(localdb)\\MSSQLLocalDB;Database=ScaleManagement;Trusted_Connection=True;MultipleActiveResultSets=true";

        var options = new DbContextOptionsBuilder<ScaleManagementDbContext>()
            .UseSqlServer(connectionString, sql => sql.MigrationsAssembly(typeof(ScaleManagementDbContext).Assembly.FullName))
            .Options;

        // Unscoped ambient context: TenantId is null, so tenant query filters are
        // inert during design-time operations.
        return new ScaleManagementDbContext(options, new AmbientTenantContext());
    }
}
