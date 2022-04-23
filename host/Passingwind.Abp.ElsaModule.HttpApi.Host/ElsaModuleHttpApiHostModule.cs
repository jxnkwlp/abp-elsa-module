using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Elsa;
using Elsa.Activities.UserTask.Extensions;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using Passingwind.Abp.ElsaModule.EntityFrameworkCore;
using Passingwind.Abp.ElsaModule.MultiTenancy;
using StackExchange.Redis;
using Swashbuckle.AspNetCore.SwaggerGen;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc.AntiForgery;
using Volo.Abp.AspNetCore.Mvc.UI.MultiTenancy;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Caching;
using Volo.Abp.Caching.StackExchangeRedis;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.SqlServer;
using Volo.Abp.Json;
using Volo.Abp.Json.SystemTextJson;
using Volo.Abp.Json.SystemTextJson.JsonConverters;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.Swashbuckle;
using Volo.Abp.TenantManagement.EntityFrameworkCore;
using Volo.Abp.VirtualFileSystem;

namespace Passingwind.Abp.ElsaModule;

[DependsOn(
    typeof(ElsaModuleApplicationModule),
    typeof(ElsaModuleEntityFrameworkCoreModule),
    typeof(ElsaModuleHttpApiModule),
    typeof(AbpAspNetCoreMvcUiMultiTenancyModule),
    typeof(AbpAutofacModule),
    typeof(AbpCachingStackExchangeRedisModule),
    typeof(AbpEntityFrameworkCoreSqlServerModule),
    typeof(AbpAuditLoggingEntityFrameworkCoreModule),
    typeof(AbpPermissionManagementEntityFrameworkCoreModule),
    typeof(AbpSettingManagementEntityFrameworkCoreModule),
    typeof(AbpTenantManagementEntityFrameworkCoreModule),
    typeof(AbpAspNetCoreSerilogModule),
    typeof(AbpSwashbuckleModule)
    )]
public class ElsaModuleHttpApiHostModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var hostingEnvironment = context.Services.GetHostingEnvironment();
        var configuration = context.Services.GetConfiguration();

        Configure<AbpDbContextOptions>(options =>
        {
            options.UseSqlServer();
        });

        Configure<AbpMultiTenancyOptions>(options =>
        {
            options.IsEnabled = MultiTenancyConsts.IsEnabled;
        });

        if (hostingEnvironment.IsDevelopment())
        {
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.ReplaceEmbeddedByPhysical<ElsaModuleDomainSharedModule>(Path.Combine(hostingEnvironment.ContentRootPath, string.Format("..{0}..{0}src{0}Passingwind.Abp.ElsaModule.Domain.Shared", Path.DirectorySeparatorChar)));
                options.FileSets.ReplaceEmbeddedByPhysical<ElsaModuleDomainModule>(Path.Combine(hostingEnvironment.ContentRootPath, string.Format("..{0}..{0}src{0}Passingwind.Abp.ElsaModule.Domain", Path.DirectorySeparatorChar)));
                options.FileSets.ReplaceEmbeddedByPhysical<ElsaModuleApplicationContractsModule>(Path.Combine(hostingEnvironment.ContentRootPath, string.Format("..{0}..{0}src{0}Passingwind.Abp.ElsaModule.Application.Contracts", Path.DirectorySeparatorChar)));
                options.FileSets.ReplaceEmbeddedByPhysical<ElsaModuleApplicationModule>(Path.Combine(hostingEnvironment.ContentRootPath, string.Format("..{0}..{0}src{0}Passingwind.Abp.ElsaModule.Application", Path.DirectorySeparatorChar)));
            });
        }

        context.Services.AddAbpSwaggerGenWithOAuth(
            configuration["AuthServer:Authority"],
            new Dictionary<string, string>
            {
                {"ElsaModule", "ElsaModule API"}
            },
            options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "ElsaModule API", Version = "v1" });
                options.DocInclusionPredicate((docName, description) => true);
                options.SchemaFilter<SwaggerEnumDescriptions>();
                // options.CustomSchemaIds(type => type.FullName);
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

                    return type.Name.RemovePostFix("Dto");
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
            options.Languages.Add(new LanguageInfo("hu", "hu", "Magyar"));
            options.Languages.Add(new LanguageInfo("pt-BR", "pt-BR", "Português"));
            options.Languages.Add(new LanguageInfo("ro-RO", "ro-RO", "Română"));
            options.Languages.Add(new LanguageInfo("ru", "ru", "Русский"));
            options.Languages.Add(new LanguageInfo("sk", "sk", "Slovak"));
            options.Languages.Add(new LanguageInfo("tr", "tr", "Türkçe"));
            options.Languages.Add(new LanguageInfo("zh-Hans", "zh-Hans", "简体中文"));
            options.Languages.Add(new LanguageInfo("zh-Hant", "zh-Hant", "繁體中文"));
            options.Languages.Add(new LanguageInfo("de-DE", "de-DE", "Deutsch"));
            options.Languages.Add(new LanguageInfo("es", "es", "Español"));
        });

        context.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = configuration["AuthServer:Authority"];
                options.RequireHttpsMetadata = Convert.ToBoolean(configuration["AuthServer:RequireHttpsMetadata"]);
                options.Audience = "ElsaModule";
            });

        Configure<AbpDistributedCacheOptions>(options =>
        {
            options.KeyPrefix = "ElsaModule:";
        });

        var dataProtectionBuilder = context.Services.AddDataProtection().SetApplicationName("ElsaModule");
        if (!hostingEnvironment.IsDevelopment())
        {
            var redis = ConnectionMultiplexer.Connect(configuration["Redis:Configuration"]);
            dataProtectionBuilder.PersistKeysToStackExchangeRedis(redis, "ElsaModule-Protection-Keys");
        }

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

#if DEBUG
        context.Services.AddAlwaysAllowAuthorization();
#endif

        context.Services.AddMemoryCache(o =>
        {
        });

        context.Services.AddHangfire(configuration =>
        {
            configuration.UseMemoryStorage();
        });
        context.Services.AddHangfireServer();

        context.Services
           .AddElsa(elsa =>
           {
               elsa.UseStore();

               elsa
                .AddConsoleActivities()
                .AddHttpActivities()
                .AddEmailActivities()
                .AddJavaScriptActivities()
                .AddUserTaskActivities()
                // .AddBlobStorageActivities()
                // .AddEntityActivities()
                // .AddFileActivities()
                .AddHangfireTemporalActivities()
                ;
           }
         );
        // context.Services.AddElsaApiEndpoints();

        context.Services.AddAbpApiVersioning(c => { });


        Configure<MvcNewtonsoftJsonOptions>((options) =>
        {
            options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver() { NamingStrategy = new CamelCaseNamingStrategy(false, true) };
        });

        //PostConfigure<AbpSystemTextJsonSerializerOptions>(options =>
        //{
        //    options.JsonSerializerOptions.Converters.RemoveAll(x => x.GetType() == typeof(AbpStringToEnumFactory));
        //});

        //PostConfigure<AbpJsonOptions>(options =>
        //{
        //});

        //PostConfigure<JsonOptions>(options =>
        //{
        //    options.JsonSerializerOptions.Converters.RemoveAll(x => x.GetType() == typeof(AbpStringToEnumFactory));
        //});

        Configure<AbpAntiForgeryOptions>(options =>
        {
            options.AutoValidate = false;
        });

    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();
        var env = context.GetEnvironment();

        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
            ForwardLimit = null,
        });

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseHsts();
        }

        // app.UseHttpsRedirection();
        app.UseCorrelationId();
        app.UseStaticFiles();
        //app.UseSpaStaticFiles();

        app.UseRouting();
        app.UseCors();

        app.UseHttpActivities();

        app.UseAuthentication();
        if (MultiTenancyConsts.IsEnabled)
        {
            app.UseMultiTenancy();
        }

        app.UseHangfireDashboard();

        app.UseAbpRequestLocalization();
        app.UseAuthorization();
        app.UseSwagger();
        app.UseAbpSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Support APP API");
            options.DisplayOperationId();
            options.DisplayRequestDuration();

            var configuration = context.GetConfiguration();
            options.OAuthClientId(configuration["AuthServer:SwaggerClientId"]);
            options.OAuthClientSecret(configuration["AuthServer:SwaggerClientSecret"]);
            options.OAuthScopes("ElsaModule");
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

//#if !DEBUG
//        app.UseSpa(c => { });
//#endif
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
}
