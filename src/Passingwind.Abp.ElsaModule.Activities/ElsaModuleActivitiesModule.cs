using Elsa.Activities.Email.Services;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Passingwind.Abp.ElsaModule.Bookmarks;
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
            context.Services.AddTransient<IUserLookupService, EmptyUserLookupService>();
            context.Services.AddTransient<IRoleLookupService, EmptyRoleLookupService>();

            context.Services.AddTransient<ICSharpEvaluator, CSharpEvaluator>();

            context.Services
                .AddJavaScriptTypeDefinitionProvider<CurrentUserTypeDefinitionProvider>()
                .AddJavaScriptTypeDefinitionProvider<CurrentTenantTypeDefinitionProvider>()
                .AddJavaScriptTypeDefinitionProvider<ClockTypeDefinitionProvider>()
                ;

            context.Services.AddBookmarkProvider<WorkflowFaultedBookmarkProvider>();
            //context.Services.AddBookmarkProvidersFrom(typeof(ElsaModuleActivitiesModule).Assembly);
            //context.Services.AddWorkflowContextProvider(typeof(ElsaModuleActivitiesModule).Assembly);

            context.Services.AddMediatR(typeof(ElsaModuleActivitiesModule).Assembly);

            context.Services.AddOptions<CSharpOptions>("Elsa:CSharp");
        }

        public override void PostConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.Replace<ISmtpService, AbpSmtpService>(ServiceLifetime.Singleton);
        }
    }
}
