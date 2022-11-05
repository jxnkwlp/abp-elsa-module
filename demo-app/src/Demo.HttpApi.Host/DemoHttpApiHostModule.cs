using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Demo.EntityFrameworkCore;
using Demo.MultiTenancy;
using Demo.Services;
using Elsa;
using Elsa.Activities.Http.JavaScript;
using Elsa.Activities.Http.OpenApi;
using Elsa.Activities.Http.Scripting.JavaScript;
using Elsa.Activities.Http.Services;
using Elsa.Activities.Sql.Extensions;
using Elsa.Activities.UserTask.Extensions;
using Elsa.Scripting.JavaScript.Options;
using Elsa.Scripting.JavaScript.Providers;
using Hangfire;
using Hangfire.Annotations;
using Hangfire.Dashboard;
using Hangfire.MemoryStorage;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using Passingwind.Abp.ElsaModule;
using Passingwind.Abp.ElsaModule.Activities;
using Passingwind.Abp.ElsaModule.MongoDB;
using Passingwind.Abp.ElsaModule.Services;
using StackExchange.Redis;
using Swashbuckle.AspNetCore.SwaggerGen;
using Volo.Abp;
using Volo.Abp.Account.Web;
using Volo.Abp.AspNetCore.MultiTenancy;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.AntiForgery;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Autofac;
using Volo.Abp.Caching;
using Volo.Abp.Caching.StackExchangeRedis;
using Volo.Abp.Identity.AspNetCore;
using Volo.Abp.Json;
using Volo.Abp.Localization;
using Volo.Abp.MailKit;
using Volo.Abp.Modularity;
using Volo.Abp.Swashbuckle;
using Volo.Abp.Timing;
using Volo.Abp.VirtualFileSystem;

namespace Demo;

[DependsOn(
    typeof(ElsaModuleActivitiesModule),
    typeof(DemoHttpApiModule),
    typeof(AbpAutofacModule),
    typeof(AbpCachingStackExchangeRedisModule),
    typeof(AbpAspNetCoreMultiTenancyModule),
    typeof(AbpIdentityAspNetCoreModule),
    typeof(AbpAccountWebModule),
    typeof(DemoApplicationModule),
    typeof(DemoEntityFrameworkCoreModule),
    typeof(AbpAspNetCoreSerilogModule),
    typeof(AbpSwashbuckleModule),
    typeof(AbpMailKitModule)
)]
// [DependsOn(typeof(ElsaModuleMongoDbModule))]
public class DemoHttpApiHostModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();
        var hostingEnvironment = context.Services.GetHostingEnvironment();

        ConfigureConventionalControllers();
        ConfigureAuthentication(context, configuration);
        ConfigureLocalization();
        ConfigureCache(configuration);
        ConfigureVirtualFileSystem(context);
        ConfigureDataProtection(context, configuration, hostingEnvironment);
        ConfigureCors(context, configuration);
        ConfigureSwaggerServices(context, configuration);
        ConfigureElsa(context, configuration);

        Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;

            options.ForwardLimit = null;
            options.KnownProxies.Clear();
            options.KnownNetworks.Clear();
        });

        context.Services.AddHangfire(config =>
        {
            if (configuration.GetValue<bool>("Redis:IsEnabled"))
            {
                config.UseRedisStorage(configuration["Redis:Configuration"], new Hangfire.Redis.RedisStorageOptions { Prefix = "Demo:Hangfire:" });
            }
            else
            {
                config.UseMemoryStorage();
            }
        });
        context.Services.AddHangfireServer(options =>
        {
            // hangfire default
            // options.SchedulePollingInterval = TimeSpan.FromSeconds(15);
        });

        context.Services.AddAbpApiVersioning(c => { });

        Configure<MvcNewtonsoftJsonOptions>((options) =>
        {
            options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver() { NamingStrategy = new CamelCaseNamingStrategy(false, true) };
        });

        Configure<AbpAntiForgeryOptions>(options =>
        {
            options.AutoValidate = false;
        });

        Configure<AbpClockOptions>(options =>
        {
            options.Kind = DateTimeKind.Utc;
        });

        Configure<AbpJsonOptions>(options =>
        {
            options.UseHybridSerializer = false;
        });

        context.Services.AddSpaStaticFiles(options =>
        {
            options.RootPath = "wwwroot/dist";
        });

        // Mediator.
        context.Services.AddMediatR(typeof(Program).Assembly);
    }

    public override void PostConfigureServices(ServiceConfigurationContext context)
    {
        // elsa issue fix
        context.Services.RemoveAll(x => x.ImplementationType == typeof(HttpTypeDefinitionProvider));
        context.Services.RemoveAll(x => x.ImplementationType == typeof(HttpEndpointTypeDefinitionRenderer));
    }

    private void ConfigureElsa(ServiceConfigurationContext context, IConfiguration configuration)
    {
        context.Services.AddAbpElsa(configure =>
        {
            configure
                .AddConsoleActivities()
                .AddHttpActivities(http =>
                {
                    http.HttpEndpointAuthorizationHandlerFactory = ActivatorUtilities.GetServiceOrCreateInstance<AuthenticationBasedHttpEndpointAuthorizationHandler>;
                }, builder =>
                {
                    builder.ConfigureHttpClient(client => { client.Timeout = TimeSpan.FromMinutes(10); });
                })
                .AddEmailActivities()
                .AddJavaScriptActivities()
                .AddUserTaskActivities()
                // .AddBlobStorageActivities()
                // .AddEntityActivities()
                // .AddFileActivities() 
                .AddHangfireTemporalActivities()
                // .AddRabbitMqActivities()
                .AddSqlServerActivities()
                ;
        });
        // context.Services.AddElsaApiEndpoints();

        Configure<ScriptOptions>(o =>
        {
            o.AllowClr = true;
        });

        context.Services.AddTransient<IUserLookupService, UserAndRoleLookupService>();
        context.Services.AddTransient<IRoleLookupService, UserAndRoleLookupService>();

        // elsa issue fix
        context.Services.AddJavaScriptTypeDefinitionProvider<HttpTypeDefinitionProviderFix>();
        context.Services.AddSingleton<IActivityTypeDefinitionRenderer, HttpEndpointTypeDefinitionRendererFix>();
    }

    private void ConfigureCache(IConfiguration configuration)
    {
        Configure<AbpDistributedCacheOptions>(options => { options.KeyPrefix = "Demo:"; });
    }

    private void ConfigureVirtualFileSystem(ServiceConfigurationContext context)
    {
        var hostingEnvironment = context.Services.GetHostingEnvironment();

        if (hostingEnvironment.IsDevelopment())
        {
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.ReplaceEmbeddedByPhysical<DemoDomainSharedModule>(
                    Path.Combine(hostingEnvironment.ContentRootPath,
                        $"..{Path.DirectorySeparatorChar}Demo.Domain.Shared"));
                options.FileSets.ReplaceEmbeddedByPhysical<DemoDomainModule>(
                    Path.Combine(hostingEnvironment.ContentRootPath,
                        $"..{Path.DirectorySeparatorChar}Demo.Domain"));
                options.FileSets.ReplaceEmbeddedByPhysical<DemoApplicationContractsModule>(
                    Path.Combine(hostingEnvironment.ContentRootPath,
                        $"..{Path.DirectorySeparatorChar}Demo.Application.Contracts"));
                options.FileSets.ReplaceEmbeddedByPhysical<DemoApplicationModule>(
                    Path.Combine(hostingEnvironment.ContentRootPath,
                        $"..{Path.DirectorySeparatorChar}Demo.Application"));
            });
        }
    }

    private void ConfigureConventionalControllers()
    {
        Configure<AbpAspNetCoreMvcOptions>(options =>
        {
            options.ConventionalControllers.Create(typeof(DemoApplicationModule).Assembly);
        });
    }

    private void ConfigureAuthentication(ServiceConfigurationContext context, IConfiguration configuration)
    {
        context.Services
            .AddAuthentication(o =>
            {
                o.DefaultScheme = IdentityConstants.ApplicationScheme;
                o.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            })
            .AddJwtBearer(options =>
            {
                options.Authority = configuration["AuthServer:Authority"];
                options.RequireHttpsMetadata = Convert.ToBoolean(configuration["AuthServer:RequireHttpsMetadata"]);
                options.Audience = "Demo";
            })
            ;
        context.Services.ConfigureApplicationCookie(optopns =>
        {
            optopns.Events.OnRedirectToLogin = (context) =>
            {
                if (context.Request.Path.StartsWithSegments("/api"))
                {
                    context.Response.StatusCode = 401;
                }
                return Task.CompletedTask;
            };
            optopns.Events.OnRedirectToAccessDenied = (context) =>
            {
                if (context.Request.Path.StartsWithSegments("/api"))
                {
                    context.Response.StatusCode = 403;
                }
                return Task.CompletedTask;
            };
        });
    }

    private static void ConfigureSwaggerServices(ServiceConfigurationContext context, IConfiguration configuration)
    {
        context.Services.AddAbpSwaggerGenWithOAuth(
            configuration["AuthServer:Authority"],
            new Dictionary<string, string>
            {
                    {"Demo", "Demo API"}
            },
            options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
                options.DocInclusionPredicate((docName, description) => true);
                options.CustomSchemaIds(type => type.FullName);
                // 
                options.DocumentFilter<HttpEndpointDocumentFilter>();
                options.SchemaFilter<SwaggerEnumDescriptions>();
                // 
                options.CustomSchemaIds(type =>
                {
                    if (type.IsGenericType)
                    {
                        var part1 = type.FullName.Substring(0, type.FullName.IndexOf("`")).RemovePostFix("Dto");
                        var part2 = string.Concat(type.GetGenericArguments().Select(x => x.Name.RemovePostFix("Dto")));

                        if (part1.EndsWith("ListResult") || part1.EndsWith("PagedResult"))
                        {
                            var temp1 = part1.Substring(0, part1.LastIndexOf("."));
                            var temp2 = part1.Substring(part1.LastIndexOf(".") + 1);
                            return $"{temp1}.{part2}{temp2}";
                        }

                        return $"{part1}.{part2}";
                    }

                    return type.FullName.RemovePostFix("Dto");
                });

                options.CustomOperationIds(e =>
                {
                    var action = e.ActionDescriptor.RouteValues["action"];
                    var controller = e.ActionDescriptor.RouteValues["controller"];
                    var method = e.HttpMethod;

                    if (action == "GetList")
                        return $"Get{controller}List";

                    if (action == "GetAllList")
                        return $"GetAll{controller}List";

                    if (action.StartsWith("GetAll"))
                        return $"GetAll{controller}{action.RemovePreFix("GetAll")}";

                    if (action == ("Get") || action == ("Create") || action == ("Update") || action == ("Delete"))
                        return action + controller;

                    if (action.StartsWith("Get"))
                        return $"Get{controller}{action.RemovePreFix("Get")}";

                    if (action.StartsWith("Create"))
                        return $"Create{controller}{action.RemovePreFix("Create")}";

                    if (action.StartsWith("Update"))
                        return $"Update{controller}{action.RemovePreFix("Update")}";

                    if (action.StartsWith("Delete"))
                        return $"Delete{controller}{action.RemovePreFix("Delete")}";

                    if (action.StartsWith("BatchDelete"))
                        return $"BatchDelete{controller}";

                    if (method == "HttpGet")
                        return action + controller;
                    else
                        return controller + action;
                });
            });
    }

    private void ConfigureLocalization()
    {
        Configure<AbpLocalizationOptions>(options =>
        {
            options.Languages.Add(new LanguageInfo("ar", "ar", "العربية"));
            options.Languages.Add(new LanguageInfo("cs", "cs", "Čeština"));
            options.Languages.Add(new LanguageInfo("en", "en", "English"));
            options.Languages.Add(new LanguageInfo("en-GB", "en-GB", "English (UK)"));
            options.Languages.Add(new LanguageInfo("fi", "fi", "Finnish"));
            options.Languages.Add(new LanguageInfo("fr", "fr", "Français"));
            options.Languages.Add(new LanguageInfo("hi", "hi", "Hindi", "in"));
            options.Languages.Add(new LanguageInfo("is", "is", "Icelandic", "is"));
            options.Languages.Add(new LanguageInfo("it", "it", "Italiano", "it"));
            options.Languages.Add(new LanguageInfo("ro-RO", "ro-RO", "Română"));
            options.Languages.Add(new LanguageInfo("hu", "hu", "Magyar"));
            options.Languages.Add(new LanguageInfo("pt-BR", "pt-BR", "Português"));
            options.Languages.Add(new LanguageInfo("ru", "ru", "Русский"));
            options.Languages.Add(new LanguageInfo("sk", "sk", "Slovak"));
            options.Languages.Add(new LanguageInfo("tr", "tr", "Türkçe"));
            options.Languages.Add(new LanguageInfo("zh-Hans", "zh-Hans", "简体中文"));
            options.Languages.Add(new LanguageInfo("zh-Hant", "zh-Hant", "繁體中文"));
            options.Languages.Add(new LanguageInfo("de-DE", "de-DE", "Deutsch", "de"));
            options.Languages.Add(new LanguageInfo("es", "es", "Español", "es"));
        });
    }

    private void ConfigureDataProtection(
        ServiceConfigurationContext context,
        IConfiguration configuration,
        IWebHostEnvironment hostingEnvironment)
    {
        var dataProtectionBuilder = context.Services.AddDataProtection().SetApplicationName("Demo");
        if (!hostingEnvironment.IsDevelopment() && configuration.GetValue<bool>("Redis:IsEnabled"))
        {
            var redis = ConnectionMultiplexer.Connect(configuration["Redis:Configuration"]);
            dataProtectionBuilder.PersistKeysToStackExchangeRedis(redis, "Demo-Protection-Keys");
        }
    }

    private void ConfigureCors(ServiceConfigurationContext context, IConfiguration configuration)
    {
        context.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder
                    .WithOrigins(
                        configuration["App:CorsOrigins"]
                            .Split(",", StringSplitOptions.RemoveEmptyEntries)
                            .Select(o => o.RemovePostFix("/"))
                            .ToArray()
                    )
                    .WithAbpExposedHeaders()
                    .SetIsOriginAllowedToAllowWildcardSubdomains()
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();
        var env = context.GetEnvironment();


        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseForwardedHeaders();
        app.UseAbpRequestLocalization();
        app.UseCorrelationId();
        app.UseStatusCodePages();
        app.UseStaticFiles();
        app.UseSpaStaticFiles();
        app.UseRouting();
        app.UseCors();
        app.UseAuthentication();
        app.UseHangfireDashboard(options: new DashboardOptions() { IgnoreAntiforgeryToken = true, Authorization = new[] { new HangfireDashboardAsyncAuthorizationFilter() } });

        if (MultiTenancyConsts.IsEnabled)
        {
            app.UseMultiTenancy();
        }

        app.UseAuthorization();

        app.UseHttpActivities();

        app.UseSwagger();
        app.UseAbpSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Demo API");

            var configuration = context.GetConfiguration();
            options.OAuthClientId(configuration["AuthServer:SwaggerClientId"]);
            options.OAuthClientSecret(configuration["AuthServer:SwaggerClientSecret"]);
            options.OAuthScopes("Demo");
        });

        app.UseAuditing();
        app.UseAbpSerilogEnrichers();

        app.UseConfiguredEndpoints();

        app.Use((context, next) =>
        {
            if (context.Request.Path.StartsWithSegments("/api"))
            {
                context.Response.StatusCode = 404;
                return Task.CompletedTask;
            }

            return next();
        });

        app.UseSpa(c => { });
    }

    public class SwaggerEnumDescriptions : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            var type = context.Type;

            if (type.IsEnum)
            {
                var names = Enum.GetNames(type);

                var values2 = new OpenApiArray();

                values2.AddRange(names.Select(x => new OpenApiObject
                {
                    ["name"] = new OpenApiString(Convert.ToInt32(Enum.Parse(type, x)).ToString()),
                    ["value"] = new OpenApiString(x),
                }));

                var values1 = new OpenApiArray();
                values1.AddRange(names.Select(x => new OpenApiString(x)));

                schema.Extensions.Add(
                    "x-enumNames",
                    values1
                );

                schema.Extensions.Add(
                    "x-ms-enum",
                    new OpenApiObject
                    {
                        ["name"] = new OpenApiString(type.Name),
                        ["modelAsString"] = new OpenApiBoolean(true),
                        ["values"] = values2,
                    }
                );
            }
        }
    }

    private class HangfireDashboardAsyncAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize([NotNull] DashboardContext context)
        {
            return context.GetHttpContext().User?.Identity?.IsAuthenticated ?? false;
        }
    }
}
