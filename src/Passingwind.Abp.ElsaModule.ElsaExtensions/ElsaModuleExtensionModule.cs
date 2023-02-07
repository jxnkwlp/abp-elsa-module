using Elsa.Activities.Email.Services;
using Elsa.Expressions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Passingwind.Abp.ElsaModule.Bookmarks;
using Passingwind.Abp.ElsaModule.Monacos.Providers;
using Passingwind.Abp.ElsaModule.Monacos.Providers.Roslyns;
using Passingwind.Abp.ElsaModule.Scripting.CSharp;
using Passingwind.Abp.ElsaModule.Scripting.CSharp.Expressions;
using Passingwind.Abp.ElsaModule.Scripting.JavaScript;
using Passingwind.Abp.ElsaModule.Services;
using Volo.Abp.Emailing;
using Volo.Abp.EventBus;
using Volo.Abp.Modularity;

namespace Passingwind.Abp.ElsaModule;

[DependsOn(
    typeof(AbpEmailingModule),
    typeof(AbpEventBusModule),
    typeof(ElsaModuleDomainModule)
)]
public class ElsaModuleExtensionModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<ElsaModuleOptions>(configure =>
        {
            configure.Builder
                .AddActivitiesFrom(typeof(ElsaModuleExtensionModule))
                ;
        });

        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(ElsaModuleExtensionModule).Assembly);
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddTransient<IUserLookupService, EmptyUserLookupService>();
        context.Services.AddTransient<IRoleLookupService, EmptyRoleLookupService>();

        // 
        context.Services
                .AddScoped<ICSharpEvaluator, CSharpEvaluator>()
                .AddScoped<IExpressionHandler, CSharpExpressionHandler>();

        context.Services
            .AddJavaScriptTypeDefinitionProvider<CurrentUserTypeDefinitionProvider>()
            .AddJavaScriptTypeDefinitionProvider<CurrentTenantTypeDefinitionProvider>()
            .AddJavaScriptTypeDefinitionProvider<ClockTypeDefinitionProvider>()
            .AddJavaScriptTypeDefinitionProvider<WorkflowFaultedTypeDefinitionProvider>()
            ;

        context.Services.AddBookmarkProvider<WorkflowFaultedBookmarkProvider>();
        //context.Services.AddBookmarkProvidersFrom(typeof(ElsaModuleActivitiesModule).Assembly);
        //context.Services.AddWorkflowContextProvider(typeof(ElsaModuleActivitiesModule).Assembly);

        context.Services.AddMediatR(typeof(ElsaModuleExtensionModule).Assembly);

        context.Services.AddOptions<CSharpScriptOptions>("Elsa:CSharp");

        // Monaco editor service
        context.Services.AddScoped<IMonacoCompletionProvider, CompletionProvider>();
        context.Services.AddScoped<IMonacoCodeAnalysisProvider, CodeAnalysisProvider>();
        context.Services.AddScoped<IMonacoHoverInfoProvider, HoverInfoProvider>();
        context.Services.AddScoped<IMonacoSignatureProvider, SignatureProvider>();
        context.Services.AddScoped<IMonacoCodeFormatterProvider, CodeFormatterProvider>();
    }

    public override void PostConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.Replace<ISmtpService, AbpSmtpService>(ServiceLifetime.Singleton);
    }
}
