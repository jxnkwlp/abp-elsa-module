using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Demo.ApiKeys;
using Demo.EntityFrameworkCore;
using Demo.MultiTenancy;
using Demo.Services;
using Elsa;
using Elsa.Activities.Http.OpenApi;
using Elsa.Activities.Http.Services;
using Elsa.Activities.Sql.Extensions;
using Elsa.Activities.UserTask.Extensions;
using Elsa.Providers.Workflows;
using Elsa.Providers.WorkflowStorage;
using Elsa.Scripting.JavaScript.Options;
using Hangfire;
using Hangfire.MemoryStorage;
using Medallion.Threading;
using Medallion.Threading.FileSystem;
using Medallion.Threading.Redis;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NodaTime;
using NodaTime.Serialization.JsonNet;
using Owl.Abp.CultureMap;
using Passingwind.Abp.ElsaModule;
using Passingwind.Abp.ElsaModule.Services;
using StackExchange.Redis;
using Storage.Net;
using Volo.Abp;
using Volo.Abp.Account.Web;
using Volo.Abp.AspNetCore.MultiTenancy;
using Volo.Abp.AspNetCore.Mvc.AntiForgery;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundJobs.Hangfire;
using Volo.Abp.BackgroundWorkers.Hangfire;
using Volo.Abp.Caching;
using Volo.Abp.Caching.StackExchangeRedis;
using Volo.Abp.DistributedLocking;
using Volo.Abp.Hangfire;
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
    typeof(ElsaModuleExtensionModule),
    typeof(DemoHttpApiModule),
    typeof(AbpAutofacModule),
    typeof(AbpCachingStackExchangeRedisModule),
    typeof(AbpAspNetCoreMultiTenancyModule),
    typeof(AbpIdentityAspNetCoreModule),
    typeof(AbpAccountWebModule),
    typeof(DemoApplicationModule),
    typeof(DemoEntityFrameworkCoreModule),
    typeof(AbpSwashbuckleModule),
    typeof(AbpMailKitModule),
    typeof(AbpDistributedLockingModule),
    typeof(AbpBackgroundJobsHangfireModule),
    typeof(AbpBackgroundWorkersHangfireModule)
)]
// [DependsOn(typeof(ElsaModuleMongoDbModule))]
public partial class DemoHttpApiHostModule : AbpModule
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
        ConfigureHangfire(context, configuration);

        context.Services.AddSingleton<IDistributedLockProvider>(sp =>
        {
            if (configuration.GetValue<bool>("Redis:IsEnabled"))
            {
                var connection = ConnectionMultiplexer.Connect(configuration["Redis:Configuration"]);
                return new RedisDistributedSynchronizationProvider(connection.GetDatabase());
            }
            else
            {
                return new FileDistributedSynchronizationProvider(new DirectoryInfo(Path.Combine(Path.GetTempPath(), "DemoElsaModuleDistributedLock")));
            }
        });

        Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;

            options.ForwardLimit = null;
            options.KnownProxies.Clear();
            options.KnownNetworks.Clear();
        });


        context.Services.AddAbpApiVersioning(c => { });

        Configure<MvcNewtonsoftJsonOptions>((options) =>
        {
            ConfigureNewtonsoftJsonSerializerSettings(options.SerializerSettings);
        });

        // Config default 'JsonSerializerSettings'
        JsonConvert.DefaultSettings = () =>
        {
            var settings = new JsonSerializerSettings();
            ConfigureNewtonsoftJsonSerializerSettings(settings);
            return settings;
        };

        Configure<AbpAntiForgeryOptions>(options =>
        {
            options.AutoValidate = true;
        });

        Configure<AbpClockOptions>(options =>
        {
            options.Kind = DateTimeKind.Utc;
        });

        Configure<AbpJsonOptions>(options =>
        {
            // options.DefaultDateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ssZ";
            options.UseHybridSerializer = false;
        });

        context.Services.AddSpaStaticFiles(options =>
        {
            options.RootPath = "wwwroot/dist";
        });

        // Mediator.
        context.Services.AddMediatR(typeof(Program).Assembly);

        context.Services.AddHealthChecks();
    }

    private static void ConfigureNewtonsoftJsonSerializerSettings(JsonSerializerSettings settings)
    {
        settings.Formatting = Formatting.None;
        settings.ContractResolver = new CamelCasePropertyNamesContractResolver() { NamingStrategy = new CamelCaseNamingStrategy(false, true) };
        settings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
        settings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
        settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        settings.NullValueHandling = NullValueHandling.Ignore;
        settings.DefaultValueHandling = DefaultValueHandling.Include;
        settings.TypeNameHandling = TypeNameHandling.None;
        settings.TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple;
        settings.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
    }

    public override void PostConfigureServices(ServiceConfigurationContext context)
    {
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
                    http.HttpEndpointWorkflowFaultHandlerFactory = ActivatorUtilities.GetServiceOrCreateInstance<AbpHttpEndpointWorkflowFaultHandler>;
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

        // Configure blob storage for blob storage workflow storage provider. 
        var hostEnvironment = context.Services.GetHostingEnvironment();
        var root = Path.Combine(hostEnvironment.ContentRootPath, "storage", "workflows");
        if (!Directory.Exists(root))
        {
            Directory.CreateDirectory(root);
        }
        Configure<BlobStorageWorkflowProviderOptions>(options => options.BlobStorageFactory = () => StorageFactory.Blobs.DirectoryFiles(root));
        Configure<BlobStorageWorkflowStorageProviderOptions>(options => options.BlobStorageFactory = () => StorageFactory.Blobs.DirectoryFiles(root));
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
        //Configure<AbpAspNetCoreMvcOptions>(options =>
        //{
        //    options.ConventionalControllers.Create(typeof(DemoApplicationModule).Assembly);
        //});
    }

    private void ConfigureAuthentication(ServiceConfigurationContext context, IConfiguration configuration)
    {
        context.Services
            .AddAuthentication(o =>
            {
                o.DefaultScheme = IdentityConstants.ApplicationScheme;
                o.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            })
            //.AddJwtBearer(options =>
            //{
            //    options.Authority = configuration["AuthServer:Authority"];
            //    options.RequireHttpsMetadata = Convert.ToBoolean(configuration["AuthServer:RequireHttpsMetadata"]);
            //    options.Audience = "Demo";
            //})
            .AddApiKey(options =>
            {
                options.KeyName = ApiKeyDefaults.ApiKeyName;
            })
            ;
        context.Services.ConfigureApplicationCookie(optopns =>
        {
            optopns.LoginPath = "/auth/login";
            optopns.Events.OnRedirectToLogin = (context) =>
            {
                if (context.Request.Path.StartsWithSegments("/api"))
                {
                    //var converter = context.HttpContext.RequestServices.GetRequiredService<IExceptionToErrorInfoConverter>();
                    // TODO
                    //context.Response.WriteAsJsonAsync(new RemoteServiceErrorResponse(new RemoteServiceErrorInfo()));
                    context.Response.StatusCode = 401;
                }
                return Task.CompletedTask;
            };
            optopns.Events.OnRedirectToAccessDenied = (context) =>
            {
                if (context.Request.Path.StartsWithSegments("/api"))
                {
                    // TODO
                    //context.Response.WriteAsJsonAsync(new RemoteServiceErrorResponse(new RemoteServiceErrorInfo()));
                    context.Response.StatusCode = 403;
                }
                return Task.CompletedTask;
            };

            optopns.ForwardDefaultSelector = (ctx) =>
            {
                // Bearer
                string authorization = ctx.Request.Headers.Authorization;
                if (!authorization.IsNullOrWhiteSpace() && authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    return JwtBearerDefaults.AuthenticationScheme;
                }

                // ApiKey
                if (ctx.Request.Headers.ContainsKey(ApiKeyDefaults.ApiKeyName) || ctx.Request.Query.ContainsKey(ApiKeyDefaults.ApiKeyName))
                {
                    return ApiKeyDefaults.AuthenticationScheme;
                }

                return null;
            };
        });

        // context.Services.ForwardIdentityAuthenticationForBearer();
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
            options.Languages.Add(new LanguageInfo("en", "en", "English"));
            options.Languages.Add(new LanguageInfo("zh-Hans", "zh-Hans", "简体中文"));
        });

        Configure<OwlCultureMapOptions>(options =>
        {
            var zhHansCultureMapInfo = new CultureMapInfo
            {
                TargetCulture = "zh-Hans",
                SourceCultures = new List<string>
                {
                    "zh", "zh-CN"
                }
            };

            options.CulturesMaps.Add(zhHansCultureMapInfo);
            options.UiCulturesMaps.Add(zhHansCultureMapInfo);
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

    private void ConfigureHangfire(ServiceConfigurationContext context, IConfiguration configuration)
    {
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

            config
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings(settings => settings.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb))
                .UseColouredConsoleLogProvider();
        });
        //context.Services.AddHangfireServer(options =>
        //{
        //    // hangfire default
        //    // options.SchedulePollingInterval = TimeSpan.FromSeconds(15);
        //    options.WorkerCount = Environment.ProcessorCount * 2;
        //}); 
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();
        var env = context.GetEnvironment();

        app.UseStatusCodePages();

        app.UseForwardedHeaders();
        app.UseOwlRequestLocalization();
        app.UseCorrelationId();

        app.UseHealthChecks("/health-check");

        app.UseStaticFiles();
        app.UseSpaStaticFiles(new StaticFileOptions()
        {
            OnPrepareResponse = (context) =>
            {
                context.Context.Response.Headers.Add("cache-control", new[] { "public, max-age=31536000" });
                context.Context.Response.Headers.Add("Expires", new[] { DateTime.UtcNow.AddYears(1).ToString("R") }); // Format RFC1123
            }
        });
        app.UseRouting();
        app.UseCors();
        app.UseAuthentication();
        app.UseHangfireDashboard(options: new DashboardOptions() { IgnoreAntiforgeryToken = true, AsyncAuthorization = new[] { new AbpHangfireAuthorizationFilter() } });

        if (MultiTenancyConsts.IsEnabled)
        {
            app.UseMultiTenancy();
        }

        app.UseUnitOfWork();

        app.UseAuthorization();

        app.UseHttpActivities();

        app.UseSwagger();
        app.UseAbpSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Demo API");
            options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
        });

        app.UseAuditing();

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

        app.Use((context, next) =>
        {
            context.Response.StatusCode = 404;
            return next();
        });
    }
}
