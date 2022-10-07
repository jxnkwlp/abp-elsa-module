using Elsa.Activities.Email.Services;
using Microsoft.Extensions.DependencyInjection;
using Passingwind.Abp.ElsaModule.Scripting.JavaScript;
using Passingwind.Abp.ElsaModule.Services;
using Volo.Abp.Emailing;
using Volo.Abp.EventBus;
using Volo.Abp.Modularity;

namespace Passingwind.Abp.ElsaModule.Activities
{
    [DependsOn(
        typeof(AbpEmailingModule),
        typeof(AbpEventBusModule)
        )]
    public class ElsaModuleActivitiesModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            PreConfigure<ElsaModuleOptions>(configure =>
            {
                configure.Builder
                    .AddActivitiesFrom(typeof(ElsaModuleActivitiesModule))
                    ;
            });
        }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.Replace<ISmtpService, AbpSmtpService>(ServiceLifetime.Transient);

            context.Services.AddTransient<IUserLookupService, EmptyUserLookupService>();
            context.Services.AddTransient<IRoleLookupService, EmptyRoleLookupService>();

            context.Services
                .AddJavaScriptTypeDefinitionProvider<CurrentUserTypeDefinitionProvider>()
                .AddJavaScriptTypeDefinitionProvider<CurrentTenantTypeDefinitionProvider>()
                .AddJavaScriptTypeDefinitionProvider<ClockTypeDefinitionProvider>()
                ;
        }
    }
}
