using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Passingwind.Abp.ElsaModule.EntityFrameworkCore;

public class ElsaModuleHttpApiHostMigrationsDbContextFactory : IDesignTimeDbContextFactory<ElsaModuleHttpApiHostMigrationsDbContext>
{
    public ElsaModuleHttpApiHostMigrationsDbContext CreateDbContext(string[] args)
    {
        var configuration = BuildConfiguration();

        var builder = new DbContextOptionsBuilder<ElsaModuleHttpApiHostMigrationsDbContext>()
            .UseSqlServer(configuration.GetConnectionString("Elsa"));

        return new ElsaModuleHttpApiHostMigrationsDbContext(builder.Options);
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false);

        return builder.Build();
    }
}
