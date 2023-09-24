using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
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
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
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
using Passingwind.Abp.ElsaModule.SystemTextJson.Converters;
using Passingwind.WorkflowApp.EntityFrameworkCore;
using Passingwind.WorkflowApp.MultiTenancy;
using Passingwind.WorkflowApp.Web.ApiKeys;
using Passingwind.WorkflowApp.Web.Services;
using StackExchange.Redis;
using Storage.Net;
using Volo.Abp;
using Volo.Abp.Account.Web;
using Volo.Abp.AspNetCore.Authentication.OpenIdConnect;
using Volo.Abp.AspNetCore.Mvc.AntiForgery;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Basic;
using Volo.Abp.Autofac;
using Volo.Abp.AutoMapper;
using Volo.Abp.BackgroundJobs.Hangfire;
using Volo.Abp.BackgroundWorkers.Hangfire;
using Volo.Abp.Caching;
using Volo.Abp.Caching.StackExchangeRedis;
using Volo.Abp.DistributedLocking;
using Volo.Abp.Hangfire;
using Volo.Abp.Identity.AspNetCore;
using Volo.Abp.Identity.Web;
using Volo.Abp.Json;
using Volo.Abp.Json.SystemTextJson;
using Volo.Abp.Localization;
using Volo.Abp.MailKit;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Swashbuckle;
using Volo.Abp.Timing;
using Volo.Abp.UI.Navigation.Urls;
using Volo.Abp.VirtualFileSystem;
using ObjectConverter = Passingwind.Abp.ElsaModule.SystemTextJson.Converters.ObjectConverter;
using TypeJsonConverter = Passingwind.Abp.ElsaModule.SystemTextJson.Converters.TypeJsonConverter;

namespace Passingwind.WorkflowApp.Web;

[DependsOn(
    typeof(ElsaModuleExtensionModule),
    typeof(WorkflowAppHttpApiModule),
    typeof(WorkflowAppApplicationModule),
     typeof(WorkflowAppEntityFrameworkCoreModule),
    // typeof(WorkflowAppMongoDbModule), 
    typeof(AbpAspNetCoreAuthenticationOpenIdConnectModule),
    typeof(AbpAspNetCoreMvcUiBasicThemeModule),
    typeof(AbpIdentityAspNetCoreModule),
    typeof(AbpAccountWebModule),
    typeof(AbpAutofacModule),
    typeof(AbpCachingStackExchangeRedisModule),
    typeof(AbpDistributedLockingModule),
    typeof(AbpIdentityWebModule),
    typeof(AbpSwashbuckleModule),
    typeof(AbpMailKitModule),
    typeof(AbpDistributedLockingModule),
    typeof(AbpBackgroundJobsHangfireModule),
    typeof(AbpBackgroundWorkersHangfireModule)
    )]
public class WorkflowAppWebModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var hostingEnvironment = context.Services.GetHostingEnvironment();
        var configuration = context.Services.GetConfiguration();

        ConfigureCache(context, configuration);
        ConfigureDistributedLocking(context, configuration);
        ConfigureLocalization();
        ConfigureDataProtection(context, configuration, hostingEnvironment);
        ConfigureUrls(configuration);
        ConfigureAuthentication(context, configuration);
        ConfigureAutoMapper();
        ConfigureVirtualFileSystem(hostingEnvironment);
        ConfigureMultiTenancy();
        ConfigureSwaggerServices(context.Services);
        ConfigureCors(context, configuration);
        ConfigureElsa(context, configuration);
        ConfigureHangfire(context, configuration);

        Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;

            options.ForwardLimit = null;
            options.KnownProxies.Clear();
            options.KnownNetworks.Clear();
        });

        Configure<JsonOptions>(options =>
        {
            options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Default;
            options.JsonSerializerOptions.Converters.Add(new TypeJsonConverter());
            options.JsonSerializerOptions.Converters.Add(new JObjectConverter());
            options.JsonSerializerOptions.Converters.Add(new JArrayConverter());
            options.JsonSerializerOptions.Converters.Add(new ObjectToDictionaryConverter());
            options.JsonSerializerOptions.Converters.Add(new ObjectConverter());
            options.JsonSerializerOptions.Converters.RemoveAll(x => x.GetType() == typeof(Volo.Abp.Json.SystemTextJson.JsonConverters.ObjectToInferredTypesConverter));
        });

        Configure<AbpSystemTextJsonSerializerOptions>(options =>
        {
            options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Default;
            options.JsonSerializerOptions.Converters.Add(new TypeJsonConverter());
            options.JsonSerializerOptions.Converters.Add(new JArrayConverter());
            options.JsonSerializerOptions.Converters.Add(new JObjectConverter());
            options.JsonSerializerOptions.Converters.Add(new ObjectToDictionaryConverter());
            options.JsonSerializerOptions.Converters.Add(new ObjectConverter());
            options.JsonSerializerOptions.Converters.RemoveAll(x => x.GetType() == typeof(Volo.Abp.Json.SystemTextJson.JsonConverters.ObjectToInferredTypesConverter));
        });

        // Config default 'JsonSerializerSettings'
        JsonConvert.DefaultSettings = () =>
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            ConfigureNewtonsoftJsonSerializerSettings(settings);
            return settings;
        };

        Configure<AbpAntiForgeryOptions>(options => options.AutoValidate = true);

        Configure<AbpClockOptions>(options => options.Kind = DateTimeKind.Utc);

        Configure<AbpJsonOptions>(options =>
        {
            // options.DefaultDateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ssZ"; 
            // options.InputDateTimeFormats = new List<string> { "yyyy'-'MM'-'dd'T'HH':'mm':'ssZ", "yyyy'-'MM'-'dd HH':'mm':'ss" };
        });

        Configure<CookiePolicyOptions>(options =>
        {
            options.Secure = Microsoft.AspNetCore.Http.CookieSecurePolicy.SameAsRequest;
            options.MinimumSameSitePolicy = Microsoft.AspNetCore.Http.SameSiteMode.Lax;
        });

        context.Services.AddResponseCompression();

        context.Services.AddSpaStaticFiles(options => options.RootPath = "wwwroot/dist");

        // Mediator.
        context.Services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(Program).Assembly));

        context.Services.AddHealthChecks();
    }

    private string GetAppName(IConfiguration configuration)
    {
        return configuration.GetValue<string>("App:Name", "WorkflowApp");
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
        settings.TypeNameHandling = TypeNameHandling.None; // As default, we want output the type name of json object
        settings.TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple;
        settings.PreserveReferencesHandling = PreserveReferencesHandling.None;
    }

    private void ConfigureCache(ServiceConfigurationContext context, IConfiguration configuration)
    {
        Configure<AbpDistributedCacheOptions>(options => options.KeyPrefix = GetAppName(configuration) + ":");
    }

    private void ConfigureDistributedLocking(ServiceConfigurationContext context, IConfiguration configuration)
    {
        context.Services.AddSingleton<IDistributedLockProvider>(sp =>
        {
            if (configuration.GetValue<bool>("Redis:IsEnabled"))
            {
                var database = configuration.GetValue<int>("Redis:DefaultDatabase", 0);
                ConnectionMultiplexer connection = ConnectionMultiplexer.Connect(configuration["Redis:Configuration"]);
                return new RedisDistributedSynchronizationProvider(connection.GetDatabase(database));
            }

            return new FileDistributedSynchronizationProvider(new DirectoryInfo(Path.Combine(Path.GetTempPath(), GetAppName(configuration), "distributedlock")));
        });
    }

    private void ConfigureUrls(IConfiguration configuration)
    {
        Configure<AppUrlOptions>(options => options.Applications["MVC"].RootUrl = configuration["App:SelfUrl"]);
    }

    private void ConfigureMultiTenancy()
    {
        Configure<AbpMultiTenancyOptions>(options => options.IsEnabled = MultiTenancyConsts.IsEnabled);
    }

    private void ConfigureAuthentication(ServiceConfigurationContext context, IConfiguration configuration)
    {
        context.Services.AddSingleton<OpenIdConnectPostConfigureOptions>();

        context.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            })
                        //.AddAbpOpenIdConnect("oidc", options =>
                        //{
                        //    options.Authority = configuration["AuthServer:Authority"];
                        //    options.RequireHttpsMetadata = Convert.ToBoolean(configuration["AuthServer:RequireHttpsMetadata"]);
                        //    options.ResponseType = OpenIdConnectResponseType.CodeIdToken;

                        //    options.ClientId = configuration["AuthServer:ClientId"];
                        //    options.ClientSecret = configuration["AuthServer:ClientSecret"];

                        //    options.UsePkce = true;
                        //    options.SaveTokens = true;
                        //    options.GetClaimsFromUserInfoEndpoint = true;

                        //    options.Scope.Add("roles");
                        //    options.Scope.Add("email");
                        //    options.Scope.Add("phone");
                        //    //options.Scope.Add("WorkflowApp");
                        //})
                        .AddApiKey(options => options.KeyName = ApiKeyDefaults.ApiKeyName)
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
                else
                {
                    context.Response.Redirect(context.RedirectUri);
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
                else
                {
                    context.Response.Redirect(context.RedirectUri);
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
    }

    private void ConfigureAutoMapper()
    {
        Configure<AbpAutoMapperOptions>(options => options.AddMaps<WorkflowAppWebModule>());
    }

    private void ConfigureVirtualFileSystem(IWebHostEnvironment hostingEnvironment)
    {
        if (hostingEnvironment.IsDevelopment())
        {
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.ReplaceEmbeddedByPhysical<WorkflowAppDomainSharedModule>(
                    Path.Combine(hostingEnvironment.ContentRootPath,
                        $"..{Path.DirectorySeparatorChar}Passingwind.WorkflowApp.Domain.Shared"));
                options.FileSets.ReplaceEmbeddedByPhysical<WorkflowAppDomainModule>(
                    Path.Combine(hostingEnvironment.ContentRootPath,
                        $"..{Path.DirectorySeparatorChar}Passingwind.WorkflowApp.Domain"));
                options.FileSets.ReplaceEmbeddedByPhysical<WorkflowAppApplicationContractsModule>(
                    Path.Combine(hostingEnvironment.ContentRootPath,
                        $"..{Path.DirectorySeparatorChar}Passingwind.WorkflowApp.Application.Contracts"));
                options.FileSets.ReplaceEmbeddedByPhysical<WorkflowAppApplicationModule>(
                    Path.Combine(hostingEnvironment.ContentRootPath,
                        $"..{Path.DirectorySeparatorChar}Passingwind.WorkflowApp.Application"));
            });
        }
    }

    private void ConfigureSwaggerServices(IServiceCollection services)
    {
        services.AddAbpSwaggerGen(
            options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Workflow App API", Version = "v1" });
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
                        string part1 = type.FullName.Substring(0, type.FullName.IndexOf("`")).RemovePostFix("Dto");
                        string part2 = string.Concat(type.GetGenericArguments().Select(x => x.Name.RemovePostFix("Dto")));

                        if (part1.EndsWith("ListResult") || part1.EndsWith("PagedResult"))
                        {
                            string temp1 = part1.Substring(0, part1.LastIndexOf("."));
                            string temp2 = part1.Substring(part1.LastIndexOf(".") + 1);
                            return $"{temp1}.{part2}{temp2}";
                        }

                        return $"{part1}.{part2}";
                    }

                    return type.FullName.RemovePostFix("Dto").Replace("Dto+", null);
                });

                options.CustomOperationIds(e =>
                {
                    string action = e.ActionDescriptor.RouteValues["action"];
                    string controller = e.ActionDescriptor.RouteValues["controller"];
                    string method = e.HttpMethod;

                    if (action == "GetList")
                        return $"Get{controller}List";

                    if (action == "GetAllList")
                        return $"GetAll{controller}List";

                    if (action.StartsWith("GetAll"))
                        return $"GetAll{controller}{action.RemovePreFix("GetAll")}";

                    if (action == "Get" || action == "Create" || action == "Update" || action == "Delete")
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
            CultureMapInfo zhHansCultureMapInfo = new CultureMapInfo
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

    private void ConfigureDataProtection(ServiceConfigurationContext context, IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
    {
        string appName = GetAppName(configuration);
        IDataProtectionBuilder dataProtectionBuilder = context.Services.AddDataProtection().SetApplicationName(appName);
        if (!hostingEnvironment.IsDevelopment() && configuration.GetValue<bool>("Redis:IsEnabled"))
        {
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(configuration["Redis:Configuration"]);
            dataProtectionBuilder.PersistKeysToStackExchangeRedis(redis, $"{appName}:Protection-Keys");
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
        string appName = GetAppName(configuration);

        context.Services.AddHangfire(config =>
        {
            if (configuration.GetValue<bool>("Redis:IsEnabled"))
            {
                config.UseRedisStorage(configuration["Redis:Configuration"], new Hangfire.Redis.RedisStorageOptions { Prefix = $"{appName}:Hangfire:" });
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
        // 
        //context.Services.AddHangfireServer(options =>
        //{
        //    // hangfire default
        //    // options.SchedulePollingInterval = TimeSpan.FromSeconds(15);
        //    options.WorkerCount = Environment.ProcessorCount * 2;
        //}); 
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
                }, builder => builder.ConfigureHttpClient(client => client.Timeout = TimeSpan.FromMinutes(10)))
                .AddEmailActivities()
                .AddJavaScriptActivities()
                .AddUserTaskActivities()
                .AddHangfireTemporalActivities()
                .AddSqlServerActivities()
                // .AddRabbitMqActivities()
                // .AddBlobStorageActivities()
                // .AddEntityActivities()
                // .AddFileActivities()  
                ;
        });

        Configure<ScriptOptions>(o => o.AllowClr = true);

        context.Services.AddTransient<IUserLookupService, UserAndRoleLookupService>();
        context.Services.AddTransient<IRoleLookupService, UserAndRoleLookupService>();

        // Configure blob storage for blob storage workflow storage provider. 
        IWebHostEnvironment hostEnvironment = context.Services.GetHostingEnvironment();
        string root = Path.Combine(hostEnvironment.ContentRootPath, "storage", "workflows");
        if (!Directory.Exists(root))
        {
            Directory.CreateDirectory(root);
        }
        Configure<BlobStorageWorkflowProviderOptions>(options => options.BlobStorageFactory = () => StorageFactory.Blobs.DirectoryFiles(root));
        Configure<BlobStorageWorkflowStorageProviderOptions>(options => options.BlobStorageFactory = () => StorageFactory.Blobs.DirectoryFiles(root));
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        IApplicationBuilder app = context.GetApplicationBuilder();
        IWebHostEnvironment env = context.GetEnvironment();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseCors();

        app.UseResponseCompression();

        // app.UseStatusCodePages();

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

        app.UseAuthentication();

        app.UseHangfireDashboard(options: new DashboardOptions() { IgnoreAntiforgeryToken = true, AsyncAuthorization = new[] { new AbpHangfireAuthorizationFilter() } });

        //if (MultiTenancyConsts.IsEnabled)
        //{
        //    app.UseMultiTenancy();
        //}

        app.UseAuthorization();

        app.UseHttpActivities();

        app.UseSwagger();

        app.UseAbpSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "WorkflowApp API");
            options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
            options.DisplayOperationId();
            options.DisplayRequestDuration();
        });

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
