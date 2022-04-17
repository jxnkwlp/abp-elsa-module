using Microsoft.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Passingwind.Abp.ElsaModule.EntityFrameworkCore;

public class ElsaModuleHttpApiHostMigrationsDbContext : AbpDbContext<ElsaModuleHttpApiHostMigrationsDbContext>
{
    public ElsaModuleHttpApiHostMigrationsDbContext(DbContextOptions<ElsaModuleHttpApiHostMigrationsDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ConfigureElsaModule();
    }
}
