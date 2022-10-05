using Elsa.Options;

namespace Passingwind.Abp.ElsaModule
{
    public class ElsaModuleOptions
    {
        public ElsaOptionsBuilder Builder { get; }

        public ElsaModuleOptions(ElsaOptionsBuilder builder)
        {
            Builder = builder;
        }

    }
}
