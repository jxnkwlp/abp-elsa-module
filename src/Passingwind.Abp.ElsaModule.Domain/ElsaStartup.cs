//using Elsa.Options;
//using Elsa.Services.Startup;
//using Microsoft.AspNetCore.Builder;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;

//namespace Passingwind.Abp.ElsaModule;

//public class ElsaStartup : IStartup
//{
//    public void ConfigureApp(IApplicationBuilder app)
//    {
//    }

//    public void ConfigureElsa(ElsaOptionsBuilder elsa, IConfiguration configuration)
//    {
//        elsa.Services.Replace<Elsa.Services.IIdGenerator, AbpElsaIdGenerator>(ServiceLifetime.Singleton);
//    }
//}
